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

        public async Task<List<BillingListItemVm>> GetPaidBills()
        {
            return await BuildBillListAsync(paidOnly: true);
        }

        public async Task<List<PendingBillQueueVm>> GetPendingBillQueueByPatientId(int patientId)
        {
            var result = await _db.Set<PendingBillQueueVm>()
                .FromSqlInterpolated(
                    $"EXEC GetPendingBillQueueByPatientId {patientId}"
                )
                .ToListAsync();

            return result;
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

                // Pending items from BillQueueDetails
                PendingItems = pendingQueue.Select(x => new BillingQueueItemVm
                {
                    BillQueueId = x.BillQueueId,
                    PatientTreatmentId = x.PatientTreatmentId,
                    ServiceID = x.ServiceID,

                    ServiceName = x.PatientTreatment?.Service?.ServiceName
                                  ?? string.Empty,

                    Quantity = x.Quantity,

                    Rate = x.PatientTreatment?.Rate ?? x.Amount,

                    DiscountPer = x.PatientTreatment?.DiscountPer ?? 0,

                    Amount = x.Amount,

                    IsProcessed = x.IsProcessed,
                    IsCancelled = x.IsCancelled
                }).ToList(),

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

        public async Task<int> CreateBill(int caseSheetId, int? userId = null)
        {
            var existingBill = await _db.Billings
                .FirstOrDefaultAsync(x => x.CaseSheetId == caseSheetId && !x.IsCancelled);

            if (existingBill != null)
                return existingBill.BillId;

            var pendingQueue = await _db.BillQueueDetails
                .Include(x => x.PatientTreatment)
                .Where(x => x.CaseSheetId == caseSheetId && !x.IsProcessed && !x.IsCancelled)
                .OrderBy(x => x.BillQueueId)
                .ToListAsync();

            if (!pendingQueue.Any())
                return 0;

            var now = DateTime.Now;
            var patientId = pendingQueue.First().PatientId;
            var grossAmount = pendingQueue.Sum(x => x.PatientTreatment?.Rate * x.PatientTreatment?.Quantity ?? 0m);
            var netAmount = pendingQueue.Sum(x => x.PatientTreatment?.Amount ?? x.Amount);
            var discountAmount = grossAmount - netAmount;
            var billNo = await GenerateBillNoAsync(now);

            var startedTransaction = _db.Database.CurrentTransaction == null;
            var tx = startedTransaction ? await _db.Database.BeginTransactionAsync() : null;

            try
            {
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

                var details = pendingQueue.Select(x => new BillingDetails
                {
                    BillId = bill.BillId,
                    PatientTreatmentId = x.PatientTreatmentId,
                    ServiceID = x.ServiceID,
                    Quantity = x.PatientTreatment?.Quantity ?? x.Quantity,
                    Rate = x.PatientTreatment?.Rate ?? 0,
                    DiscountPer = x.PatientTreatment?.DiscountPer ?? 0,
                    Amount = x.PatientTreatment?.Amount ?? x.Amount,
                    CreatedDate = now
                }).ToList();

                _db.BillingDetails.AddRange(details);

                foreach (var queue in pendingQueue)
                {
                    queue.IsProcessed = true;
                    queue.ProcessedBillId = bill.BillId;
                    queue.ProcessedDate = now;

                    if (queue.PatientTreatment != null)
                    {
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

                await _db.SaveChangesAsync();

                if (startedTransaction && tx != null)
                {
                    await tx.CommitAsync();
                }

                return bill.BillId;
            }
            catch
            {
                if (startedTransaction && tx != null)
                {
                    await tx.RollbackAsync();
                }

                throw;
            }
        }

        public async Task SavePayment(BillingPaymentVm model, int? userId = null)
        {
            var bill = await _db.Billings.FirstOrDefaultAsync(x => x.BillId == model.BillId);

            if (bill == null)
                throw new InvalidOperationException("Bill not found.");

            if (bill.IsCancelled)
                throw new InvalidOperationException("Cancelled bill cannot accept payment.");

            if (model.Amount <= 0)
                throw new InvalidOperationException("Payment amount must be greater than zero.");

            if (model.Amount > bill.BalanceAmount)
                throw new InvalidOperationException("Payment amount cannot exceed the current balance.");

            var now = DateTime.Now;
            var previousBalance = bill.BalanceAmount;

            var payment = new PaymentDetail
            {
                BillId = bill.BillId,
                PaymentMode = model.PaymentMode,
                Amount = model.Amount,
                TransactionNo = model.TransactionNo,
                PaymentDate = now,
                ReceivedBy = userId,
                Remarks = model.Remarks
            };

            bill.PaidAmount += model.Amount;
            bill.BalanceAmount = Math.Max(0, bill.NetAmount - bill.PaidAmount);
            bill.IsPaid = bill.BalanceAmount == 0;

            _db.PaymentDetails.Add(payment);
            _db.BillingAuditLogs.Add(new BillingAuditLog
            {
                BillId = bill.BillId,
                ActionType = "PAYMENT",
                OldValue = previousBalance.ToString("0.00"),
                NewValue = model.Amount.ToString("0.00"),
                ActionBy = userId,
                ActionDate = now
            });

            await _db.SaveChangesAsync();
        }

        public async Task CancelBill(BillingCancelVm model, int? userId = null)
        {
            var bill = await _db.Billings
                .Include(x => x.BillingDetails)
                .FirstOrDefaultAsync(x => x.BillId == model.BillId);

            if (bill == null)
                throw new InvalidOperationException("Bill not found.");

            if (bill.IsCancelled)
                return;

            var now = DateTime.Now;

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
        }

        public async Task CancelService(BillingServiceCancelVm model, int? userId = null)
        {
            var detail = await _db.BillingDetails
                .Include(x => x.Billing)
                .FirstOrDefaultAsync(x => x.BillDetailId == model.BillDetailId);

            if (detail == null)
                throw new InvalidOperationException("Bill detail not found.");

            if (detail.IsCancelled)
                return;

            var now = DateTime.Now;
            detail.IsCancelled = true;
            detail.CancelReason = model.CancelReason;
            detail.CancelledBy = userId;
            detail.CancelledDate = now;

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
            bill.BalanceAmount = Math.Max(0, bill.NetAmount - bill.PaidAmount);
            bill.IsPaid = bill.BalanceAmount == 0 && !bill.IsCancelled;
            bill.RefundAmount = Math.Max(0, bill.PaidAmount - bill.NetAmount);

            _db.BillingAuditLogs.Add(new BillingAuditLog
            {
                BillId = bill.BillId,
                ActionType = "CANCEL SERVICE",
                OldValue = null,
                NewValue = model.CancelReason,
                ActionBy = userId,
                ActionDate = now
            });

            await _db.SaveChangesAsync();
        }

        private async Task<List<BillingListItemVm>> BuildBillListAsync(bool paidOnly)
        {
            var query = from bill in _db.Billings.AsNoTracking()
                        join patient in _db.OPDPatientRegistrations.AsNoTracking()
                            on bill.PatientId equals patient.PatientId
                        where !bill.IsCancelled
                              && (paidOnly ? bill.IsPaid : !bill.IsPaid)
                        orderby bill.BillDate descending
                        select new BillingListItemVm
                        {
                            BillId = bill.BillId,
                            BillNo = bill.BillNo,
                            CaseSheetId = bill.CaseSheetId,
                            PatientId = bill.PatientId,
                            PatientName = patient.PatientName,
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
            var query = from queue in _db.BillQueueDetails.AsNoTracking()
                        join patient in _db.OPDPatientRegistrations.AsNoTracking()
                            on queue.PatientId equals patient.PatientId
                        where !queue.IsCancelled
                              && !queue.IsProcessed
                        orderby queue.CreatedDate descending
                        select new BillingListItemVm
                        {
                            BillId = 0,
                            BillQueueId = queue.BillQueueId,
                            BillNo = null,
                            CaseSheetId = queue.CaseSheetId,
                            PatientId = queue.PatientId,
                            PatientName = patient.PatientName,

                            GrossAmount = queue.Amount,
                            DiscountAmount = 0,
                            NetAmount = queue.Amount,
                            PaidAmount = 0,
                            BalanceAmount = queue.Amount,
                            RefundAmount = 0,

                            IsPaid = false,
                            IsCancelled = queue.IsCancelled,
                            BillDate = queue.CreatedDate
                        };

            return await query.ToListAsync();
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
