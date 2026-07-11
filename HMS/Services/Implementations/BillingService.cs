using HMS.Data;
using HMS.Entities;
using HMS.Entities.BillingDetails;
using HMS.Models;
using HMS.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HMS.Services.Implementations
{
    public class BillingService : IBillingService
    {
        private readonly HmsDbContext _db;
        private readonly IPatientService _patientService;

        public BillingService(HmsDbContext db, IPatientService patientService)
        {
            _db = db;
            _patientService = patientService;
        }

        public async Task<List<BillingListItemVm>> GetUnpaidBills()
        {
            return await BuildUnpaidBillQueueListAsync();
        }

        public async Task<List<BillingListItemVm>> GetPaidBills(int? deptId = null, string? patient = null, string? billNo = null, DateTime? billDate = null)
        {
            return await BuildBillListAsync(paidOnly: true, deptId, patient, billNo, billDate);
        }

        public async Task<List<PendingBillQueueVm>> GetPendingBillQueueByPatientId(int patientId)
        {
            return await BuildPendingQueueQuery()
                .Where(x => x.PatientId == patientId)
                .OrderBy(x => x.CreatedDate)
                .ToListAsync();
        }

        public async Task<BillingViewVm> GetPendingBillDetails(int patientId, int caseSheetId, int deptId)
        {
            var pendingQueue = await _db.BillQueueDetails
                .Include(x => x.PatientTreatment)
                    .ThenInclude(x => x!.Service)
                .Include(x => x.PatientTreatment)
                    .ThenInclude(x => x!.Department)
                .Where(x => x.PatientId == patientId
                            && x.CaseSheetId == caseSheetId
                            && x.DeptId == deptId
                            && !x.IsProcessed
                            && !x.IsCancelled
                            && x.PatientTreatment != null
                            && !x.PatientTreatment.IsBilled
                            && !x.PatientTreatment.IsCancelled)
                .OrderBy(x => x.BillQueueId)
                .AsNoTracking()
                .ToListAsync();

            var patient = patientId > 0
                ? await _patientService.GetPatientDetails(patientId)
                : null;

            return new BillingViewVm
            {
                Patient = patient,
                PatientId = patientId,
                CaseSheetId = caseSheetId,
                DeptId = deptId,
                DepartmentName = pendingQueue.FirstOrDefault()?.PatientTreatment?.Department?.DeptName ?? string.Empty,
                PendingItems = pendingQueue.Select(ToQueueItemVm).ToList()
            };
        }

        public async Task<BillingViewVm> GetBillDetails(int billId)
        {
            var billing = await _db.Billings
                .Include(x => x.BillingDetails)
                    .ThenInclude(x => x.Service)
                .Include(x => x.BillingDetails)
                    .ThenInclude(x => x.PatientTreatment)
                        .ThenInclude(x => x!.Department)
                .Include(x => x.Payments)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.BillId == billId);

            if (billing == null)
                return new BillingViewVm();

            var patient = await _patientService.GetPatientDetails(billing.PatientId);
            var firstDetail = billing.BillingDetails.FirstOrDefault();

            return new BillingViewVm
            {
                Billing = billing,
                Patient = patient,
                PatientId = billing.PatientId,
                CaseSheetId = billing.CaseSheetId,
                DeptId = firstDetail?.PatientTreatment?.DeptId ?? 0,
                DepartmentName = firstDetail?.PatientTreatment?.Department?.DeptName ?? string.Empty,
                BillingDetails = billing.BillingDetails
                    .OrderBy(x => x.BillDetailId)
                    .ToList(),
                Payments = billing.Payments
                    .OrderByDescending(x => x.PaymentDate)
                    .ToList()
            };
        }

        public async Task<BillingViewVm> GetBillDetailsByCaseSheetId(int caseSheetId)
        {
            var billing = await _db.Billings
                .Include(x => x.BillingDetails)
                    .ThenInclude(x => x.Service)
                .Include(x => x.Payments)
                .FirstOrDefaultAsync(x => x.CaseSheetId == caseSheetId && !x.IsCancelled);

            var pendingQueue = await _db.BillQueueDetails
                .Include(x => x.PatientTreatment)
                    .ThenInclude(x => x!.Service)
                .Include(x => x.PatientTreatment)
                    .ThenInclude(x => x!.Department)
                .Where(x => x.CaseSheetId == caseSheetId
                            && !x.IsProcessed
                            && !x.IsCancelled)
                .OrderBy(x => x.BillQueueId)
                .ToListAsync();

            // Get patient from either Billing or Queue
            var patientId = billing?.PatientId
                            ?? pendingQueue.FirstOrDefault()?.PatientId
                            ?? 0;

            var patient = patientId > 0
                            ? await _patientService.GetPatientDetails(patientId)
                            : null;

            return new BillingViewVm
            {
                Billing = billing,
                Patient = patient,
                CaseSheetId = caseSheetId,
                PatientId = patientId,
                DeptId = pendingQueue.FirstOrDefault()?.DeptId ?? 0,
                DepartmentName = pendingQueue.FirstOrDefault()?.PatientTreatment?.Department?.DeptName ?? string.Empty,

                // Pending items from BillQueueDetails
                PendingItems = pendingQueue.Select(ToQueueItemVm).ToList(),

                // Existing billed items
                BillingDetails = billing?.BillingDetails
                    .OrderBy(x => x.BillDetailId)
                    .ToList()
                    ?? new List<BillingDetails>(),

                // Existing payments
                Payments = billing?.Payments
                    .OrderByDescending(x => x.PaymentDate)
                    .ToList()
                    ?? new List<PaymentDetail>()
            };
        }

        public async Task<int> SavePayment(BillingPaymentVm model, int? userId = null)
        {
            var paymentLines = NormalizePaymentLines(model);
            var totalPayment = paymentLines.Sum(x => x.Amount);

            if (totalPayment <= 0)
                throw new InvalidOperationException("Payment amount must be greater than zero.");

            var now = DateTime.Now;

            await using var tx = await _db.Database.BeginTransactionAsync();

            try
            {
                Billing bill;
                List<BillQueueDetails> pendingQueue = new();

                if (model.BillId > 0)
                {
                    bill = await _db.Billings.FirstOrDefaultAsync(x => x.BillId == model.BillId)
                        ?? throw new InvalidOperationException("Bill not found.");

                    if (bill.IsCancelled)
                        throw new InvalidOperationException("Cancelled bill cannot accept payment.");
                }
                else
                {
                    pendingQueue = await LoadPendingQueueForPayment(model.PatientId, model.CaseSheetId, model.DeptId);

                    if (!pendingQueue.Any())
                        throw new InvalidOperationException("No pending services were found for billing.");

                    var existingBill = await _db.Billings
                        .AnyAsync(x => x.PatientId == model.PatientId
                                       && x.CaseSheetId == model.CaseSheetId
                                       && !x.IsCancelled
                                       && x.BillingDetails.Any(d => !d.IsCancelled
                                                                    && d.PatientTreatment != null
                                                                    && d.PatientTreatment.DeptId == model.DeptId));

                    if (existingBill)
                        throw new InvalidOperationException("A bill already exists for this pending group.");

                    bill = await CreateBillFromQueueAsync(pendingQueue, now, userId);
                }

                if (model.BillId == 0 && Math.Abs(totalPayment - bill.NetAmount) > 0.01m)
                    throw new InvalidOperationException("Payment total must match the bill net amount.");

                if (totalPayment > bill.BalanceAmount)
                    throw new InvalidOperationException("Payment amount cannot exceed the current balance.");

                var previousBalance = bill.BalanceAmount;

                _db.PaymentDetails.AddRange(paymentLines.Select(x => new PaymentDetail
                {
                    BillId = bill.BillId,
                    PaymentMode = x.PaymentMode,
                    Amount = x.Amount,
                    TransactionNo = x.TransactionNo,
                    PaymentDate = now,
                    ReceivedBy = userId,
                    Remarks = x.Remarks
                }));

                bill.PaidAmount = Math.Round(bill.PaidAmount + totalPayment, 2);
                bill.BalanceAmount = Math.Round(Math.Max(0, bill.NetAmount - bill.PaidAmount), 2);
                bill.IsPaid = bill.BalanceAmount == 0;

                _db.BillingAuditLogs.Add(new BillingAuditLog
                {
                    BillId = bill.BillId,
                    ActionType = "PAYMENT",
                    OldValue = previousBalance.ToString("0.00"),
                    NewValue = totalPayment.ToString("0.00"),
                    ActionBy = userId,
                    ActionDate = now
                });

                await _db.SaveChangesAsync();
                await tx.CommitAsync();

                return bill.BillId;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task CancelBill(BillingCancelVm model, int? userId = null)
        {
            var bill = await _db.Billings
                .Include(x => x.BillingDetails)
                    .ThenInclude(x => x.PatientTreatment)
                .Include(x => x.Payments)
                .FirstOrDefaultAsync(x => x.BillId == model.BillId);

            if (bill == null)
                throw new InvalidOperationException("Bill not found.");

            if (bill.IsCancelled)
                return;

            var now = DateTime.Now;
            await using var tx = await _db.Database.BeginTransactionAsync();

            try
            {
                bill.IsCancelled = true;
                bill.CancelReason = model.CancelReason;
                bill.CancelledBy = userId;
                bill.CancelledDate = now;

                foreach (var detail in bill.BillingDetails)
                {
                    detail.IsCancelled = true;
                    detail.CancelReason = model.CancelReason;
                    detail.CancelledBy = userId;
                    detail.CancelledDate = now;

                    if (detail.PatientTreatment != null)
                    {
                        detail.PatientTreatment.IsBilled = false;
                        detail.PatientTreatment.IsCancelled = false;
                    }
                }

                var treatmentIds = bill.BillingDetails.Select(x => x.PatientTreatmentId).ToList();
                var queues = await _db.BillQueueDetails
                    .Where(x => treatmentIds.Contains(x.PatientTreatmentId)
                                && x.ProcessedBillId == bill.BillId)
                    .ToListAsync();

                foreach (var queue in queues)
                {
                    queue.IsProcessed = false;
                    queue.ProcessedBillId = null;
                    queue.ProcessedDate = null;
                }

                if (bill.PaidAmount > 0)
                {
                    bill.RefundAmount = bill.PaidAmount;
                    _db.PaymentDetails.Add(new PaymentDetail
                    {
                        BillId = bill.BillId,
                        PaymentMode = "REFUND",
                        Amount = 0,
                        IsRefund = true,
                        RefundAmount = bill.PaidAmount,
                        PaymentDate = now,
                        ReceivedBy = userId,
                        Remarks = model.CancelReason
                    });
                }

                _db.BillingAuditLogs.Add(new BillingAuditLog
                {
                    BillId = bill.BillId,
                    ActionType = "CANCEL BILL",
                    OldValue = null,
                    NewValue = model.CancelReason,
                    ActionBy = userId,
                    ActionDate = now
                });

                await _db.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task CancelService(BillingServiceCancelVm model, int? userId = null)
        {
            var detail = await _db.BillingDetails
                .Include(x => x.Billing)
                .Include(x => x.PatientTreatment)
                .FirstOrDefaultAsync(x => x.BillDetailId == model.BillDetailId);

            if (detail == null)
                throw new InvalidOperationException("Bill detail not found.");

            if (detail.IsCancelled)
                return;

            var now = DateTime.Now;
            await using var tx = await _db.Database.BeginTransactionAsync();

            try
            {
                detail.IsCancelled = true;
                detail.CancelReason = model.CancelReason;
                detail.CancelledBy = userId;
                detail.CancelledDate = now;

                if (detail.PatientTreatment != null)
                {
                    detail.PatientTreatment.IsBilled = false;
                    detail.PatientTreatment.IsCancelled = false;
                }

                var queue = await _db.BillQueueDetails
                    .FirstOrDefaultAsync(x => x.PatientTreatmentId == detail.PatientTreatmentId
                                              && x.ProcessedBillId == detail.BillId);

                if (queue != null)
                {
                    queue.IsProcessed = false;
                    queue.ProcessedBillId = null;
                    queue.ProcessedDate = null;
                }

                var bill = detail.Billing ?? await _db.Billings.FirstAsync(x => x.BillId == detail.BillId);
                var remainingDetails = await _db.BillingDetails
                    .Where(x => x.BillId == bill.BillId && !x.IsCancelled)
                    .ToListAsync();

                var grossAmount = remainingDetails.Sum(x => x.Rate * x.Quantity);
                var netAmount = remainingDetails.Sum(x => x.Amount);
                var discountAmount = grossAmount - netAmount;

                bill.GrossAmount = Math.Round(grossAmount, 2);
                bill.DiscountAmount = Math.Round(discountAmount, 2);
                bill.NetAmount = Math.Round(netAmount, 2);
                bill.BalanceAmount = Math.Round(Math.Max(0, bill.NetAmount - bill.PaidAmount), 2);
                bill.IsPaid = bill.BalanceAmount == 0 && !bill.IsCancelled;
                bill.RefundAmount = Math.Round(Math.Max(0, bill.PaidAmount - bill.NetAmount), 2);

                if (bill.RefundAmount > 0)
                {
                    _db.PaymentDetails.Add(new PaymentDetail
                    {
                        BillId = bill.BillId,
                        PaymentMode = "REFUND",
                        Amount = 0,
                        IsRefund = true,
                        RefundAmount = bill.RefundAmount,
                        PaymentDate = now,
                        ReceivedBy = userId,
                        Remarks = model.CancelReason
                    });
                }

                _db.BillingAuditLogs.Add(new BillingAuditLog
                {
                    BillId = bill.BillId,
                    ActionType = "CANCEL SERVICE",
                    OldValue = detail.Amount.ToString("0.00"),
                    NewValue = model.CancelReason,
                    ActionBy = userId,
                    ActionDate = now
                });

                await _db.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        private async Task<List<BillingListItemVm>> BuildBillListAsync(
            bool paidOnly,
            int? deptId = null,
            string? patientFilter = null,
            string? billNoFilter = null,
            DateTime? billDate = null)
        {
            var query = from bill in _db.Billings.AsNoTracking()
                        join patient in _db.OPDPatientRegistrations.AsNoTracking()
                            on bill.PatientId equals patient.PatientId
                        where !bill.IsCancelled
                              && (paidOnly ? bill.IsPaid : !bill.IsPaid)
                              && (!deptId.HasValue || bill.BillingDetails.Any(d => d.PatientTreatment != null && d.PatientTreatment.DeptId == deptId.Value))
                              && (string.IsNullOrWhiteSpace(patientFilter) || patient.PatientName.Contains(patientFilter))
                              && (string.IsNullOrWhiteSpace(billNoFilter) || bill.BillNo.Contains(billNoFilter))
                              && (!billDate.HasValue || bill.BillDate.Date == billDate.Value.Date)
                        orderby bill.BillDate descending
                        select new BillingListItemVm
                        {
                            BillId = bill.BillId,
                            BillNo = bill.BillNo,
                            CaseSheetId = bill.CaseSheetId,
                            PatientId = bill.PatientId,
                            PatientName = patient.PatientName,
                            OpNo = patient.OpNo ?? string.Empty,
                            GrossAmount = bill.GrossAmount,
                            DiscountAmount = bill.DiscountAmount,
                            NetAmount = bill.NetAmount,
                            PaidAmount = bill.PaidAmount,
                            BalanceAmount = bill.BalanceAmount,
                            RefundAmount = bill.RefundAmount,
                            IsPaid = bill.IsPaid,
                            IsCancelled = bill.IsCancelled,
                            BillDate = bill.BillDate
                        };

            return await query.ToListAsync();
        }

        private async Task<List<BillingListItemVm>> BuildUnpaidBillQueueListAsync()
        {
            var pending = await BuildPendingQueueQuery().ToListAsync();

            return pending
                .GroupBy(x => new
                {
                    x.PatientId,
                    x.CaseSheetId,
                    x.DeptId,
                    x.PatientName,
                    x.OpNo,
                    x.DeptName
                })
                .Select(g =>
                {
                    var grossAmount = g.Sum(x => x.Rate * x.Quantity);
                    var netAmount = g.Sum(x => x.Amount);

                    return new BillingListItemVm
                    {
                        BillId = 0,
                        BillQueueId = g.Min(x => x.BillQueueId),
                        BillNo = string.Empty,
                        CaseSheetId = g.Key.CaseSheetId,
                        PatientId = g.Key.PatientId,
                        DeptId = g.Key.DeptId,
                        PatientName = g.Key.PatientName,
                        OpNo = g.Key.OpNo,
                        DeptName = g.Key.DeptName,
                        PendingServiceCount = g.Count(),
                        GrossAmount = Math.Round(grossAmount, 2),
                        DiscountAmount = Math.Round(grossAmount - netAmount, 2),
                        NetAmount = Math.Round(netAmount, 2),
                        PaidAmount = 0,
                        BalanceAmount = Math.Round(netAmount, 2),
                        RefundAmount = 0,
                        IsPaid = false,
                        IsCancelled = false,
                        BillDate = g.Min(x => x.CreatedDate)
                    };
                })
                .OrderByDescending(x => x.BillDate)
                .ToList();
        }

        private IQueryable<PendingBillQueueVm> BuildPendingQueueQuery()
        {
            return from queue in _db.BillQueueDetails.AsNoTracking()
                   join treatment in _db.PatientTreatments.AsNoTracking()
                       on queue.PatientTreatmentId equals treatment.PatientTreatmentId
                   join patient in _db.OPDPatientRegistrations.AsNoTracking()
                       on queue.PatientId equals patient.PatientId
                   join department in _db.MASDepartments.AsNoTracking()
                       on queue.DeptId equals department.DeptId
                   join service in _db.TreatmentServices.AsNoTracking()
                       on queue.ServiceID equals service.ServiceID
                   where !queue.IsCancelled
                         && !queue.IsProcessed
                         && !treatment.IsCancelled
                         && !treatment.IsBilled
                   select new PendingBillQueueVm
                   {
                       BillQueueId = queue.BillQueueId,
                       CaseSheetId = queue.CaseSheetId,
                       PatientId = queue.PatientId,
                       PatientName = patient.PatientName,
                       OpNo = patient.OpNo ?? string.Empty,
                       PatientTreatmentId = queue.PatientTreatmentId,
                       DeptId = queue.DeptId,
                       DeptName = department.DeptName,
                       ServiceID = queue.ServiceID,
                       ServiceName = service.ServiceName,
                       Quantity = treatment.Quantity,
                       Rate = treatment.Rate,
                       DiscountPer = treatment.DiscountPer,
                       Amount = treatment.Amount,
                       IsProcessed = queue.IsProcessed,
                       ProcessedBillId = queue.ProcessedBillId,
                       CreatedDate = queue.CreatedDate
                   };
        }

        private static BillingQueueItemVm ToQueueItemVm(BillQueueDetails queue)
        {
            var treatment = queue.PatientTreatment;

            return new BillingQueueItemVm
            {
                BillQueueId = queue.BillQueueId,
                PatientTreatmentId = queue.PatientTreatmentId,
                ServiceID = queue.ServiceID,
                ServiceName = treatment?.Service?.ServiceName ?? string.Empty,
                Quantity = treatment?.Quantity ?? queue.Quantity,
                Rate = treatment?.Rate ?? 0,
                DiscountPer = treatment?.DiscountPer ?? 0,
                Amount = treatment?.Amount ?? queue.Amount,
                IsProcessed = queue.IsProcessed,
                IsCancelled = queue.IsCancelled
            };
        }

        private async Task<List<BillQueueDetails>> LoadPendingQueueForPayment(int patientId, int caseSheetId, int deptId)
        {
            return await _db.BillQueueDetails
                .Include(x => x.PatientTreatment)
                    .ThenInclude(x => x!.Service)
                .Where(x => x.PatientId == patientId
                            && x.CaseSheetId == caseSheetId
                            && x.DeptId == deptId
                            && !x.IsProcessed
                            && !x.IsCancelled
                            && x.PatientTreatment != null
                            && !x.PatientTreatment.IsBilled
                            && !x.PatientTreatment.IsCancelled)
                .OrderBy(x => x.BillQueueId)
                .ToListAsync();
        }

        private async Task<Billing> CreateBillFromQueueAsync(List<BillQueueDetails> pendingQueue, DateTime now, int? userId)
        {
            var patientId = pendingQueue.First().PatientId;
            var caseSheetId = pendingQueue.First().CaseSheetId;
            var billLines = pendingQueue.Select(x =>
            {
                var quantity = x.PatientTreatment?.Quantity ?? x.Quantity;
                var rate = x.PatientTreatment?.Rate ?? 0;
                var discountPer = x.PatientTreatment?.DiscountPer ?? 0;
                var amount = CalculateAmount(quantity, rate, discountPer);

                return new
                {
                    Queue = x,
                    Quantity = quantity,
                    Rate = rate,
                    DiscountPer = discountPer,
                    Amount = amount
                };
            }).ToList();

            var grossAmount = billLines.Sum(x => x.Rate * x.Quantity);
            var netAmount = billLines.Sum(x => x.Amount);
            var discountAmount = grossAmount - netAmount;
            var billNo = await GenerateBillNoAsync(now);

            var bill = new Billing
            {
                BillNo = billNo,
                CaseSheetId = caseSheetId,
                PatientId = patientId,
                GrossAmount = Math.Round(grossAmount, 2),
                DiscountAmount = Math.Round(discountAmount, 2),
                NetAmount = Math.Round(netAmount, 2),
                PaidAmount = 0,
                BalanceAmount = Math.Round(netAmount, 2),
                RefundAmount = 0,
                IsPaid = false,
                IsCancelled = false,
                BillDate = now,
                CreatedBy = userId,
                CreatedDate = now
            };

            _db.Billings.Add(bill);
            await _db.SaveChangesAsync();

            var details = billLines.Select(x => new BillingDetails
            {
                BillId = bill.BillId,
                PatientTreatmentId = x.Queue.PatientTreatmentId,
                ServiceID = x.Queue.ServiceID,
                Quantity = x.Quantity,
                Rate = x.Rate,
                DiscountPer = x.DiscountPer,
                Amount = x.Amount,
                CreatedDate = now
            }).ToList();

            _db.BillingDetails.AddRange(details);

            foreach (var queue in pendingQueue)
            {
                var line = billLines.First(x => x.Queue.BillQueueId == queue.BillQueueId);
                queue.Quantity = line.Quantity;
                queue.Amount = line.Amount;
                queue.IsProcessed = true;
                queue.ProcessedBillId = bill.BillId;
                queue.ProcessedDate = now;

                if (queue.PatientTreatment != null)
                {
                    queue.PatientTreatment.Amount = CalculateAmount(
                        queue.PatientTreatment.Quantity,
                        queue.PatientTreatment.Rate,
                        queue.PatientTreatment.DiscountPer);
                    queue.PatientTreatment.IsBilled = true;
                }
            }

            _db.BillingAuditLogs.Add(new BillingAuditLog
            {
                BillId = bill.BillId,
                ActionType = "CREATE BILL",
                OldValue = null,
                NewValue = $"Bill generated with {details.Count} line item(s).",
                ActionBy = userId,
                ActionDate = now
            });

            return bill;
        }

        private static decimal CalculateAmount(decimal quantity, decimal rate, decimal discountPer)
        {
            var gross = quantity * rate;
            var discount = gross * Math.Clamp(discountPer, 0, 100) / 100m;
            return Math.Round(gross - discount, 2);
        }

        private static List<BillingPaymentLineVm> NormalizePaymentLines(BillingPaymentVm model)
        {
            var lines = model.Payments?
                .Where(x => !string.IsNullOrWhiteSpace(x.PaymentMode) && x.Amount > 0)
                .Select(x => new BillingPaymentLineVm
                {
                    PaymentMode = x.PaymentMode.Trim(),
                    Amount = Math.Round(x.Amount, 2),
                    TransactionNo = x.TransactionNo,
                    Remarks = x.Remarks
                })
                .ToList() ?? new List<BillingPaymentLineVm>();

            if (lines.Any())
                return lines;

            if (!string.IsNullOrWhiteSpace(model.PaymentMode) && model.Amount > 0)
            {
                lines.Add(new BillingPaymentLineVm
                {
                    PaymentMode = model.PaymentMode.Trim(),
                    Amount = Math.Round(model.Amount, 2),
                    TransactionNo = model.TransactionNo,
                    Remarks = model.Remarks
                });
            }

            return lines;
        }

        private async Task<string> GenerateBillNoAsync(DateTime now)
        {
            var prefix = $"BILL-{now:yyyy}";

            var latestBillNo = await _db.Billings
                .AsNoTracking()
                .Where(x => x.BillNo.StartsWith(prefix))
                .OrderByDescending(x => x.BillId)
                .Select(x => x.BillNo)
                .FirstOrDefaultAsync();

            var nextNumber = 1;

            if (!string.IsNullOrWhiteSpace(latestBillNo))
            {
                var suffix = latestBillNo.Substring(prefix.Length);
                if (int.TryParse(suffix, out var parsed))
                {
                    nextNumber = parsed + 1;
                }
            }

            return $"{prefix}{nextNumber:D5}";
        }
    }
}
