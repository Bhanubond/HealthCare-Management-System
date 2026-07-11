using System.Text.Json;
using HMS.Data;
using HMS.Entities;
using HMS.Entities.BillingDetails;
using HMS.Models;
using HMS.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HMS.Services.Implementations
{
    public class PatientTreatmentService : IPatientTreatmentService
    {
        private readonly HmsDbContext _context;

        public PatientTreatmentService(HmsDbContext context)
        {
            _context = context;
        }

        public async Task<List<TreatmentServices>> GetDepartmentServices(int deptId)
        {
            return await _context.TreatmentServices
                .AsNoTracking()
                .Where(x => x.DeptId == deptId && x.IsActive)
                .OrderBy(x => x.ServiceName)
                .ToListAsync();
        }

        public async Task SavePatientTreatments(PatientTreatmentVM model)
        {
            var items = ResolveItems(model);

            if (!items.Any())
                return;

            var now = DateTime.Now;
            var treatments = new List<PatientTreatment>();
            var serviceIds = items
                .Where(x => x.TreatmentId == 0 && x.ServiceID > 0)
                .Select(x => x.ServiceID)
                .Distinct()
                .ToList();

            var services = await _context.TreatmentServices
                .AsNoTracking()
                .Where(x => serviceIds.Contains(x.ServiceID) && x.DeptId == model.DeptId && x.IsActive)
                .ToDictionaryAsync(x => x.ServiceID);

            foreach (var item in items)
            {
                if (item.TreatmentId > 0)
                    continue;

                if (item.ServiceID <= 0)
                    continue;

                if (!services.TryGetValue(item.ServiceID, out var service))
                    continue;

                var duplicateExists = await _context.PatientTreatments
                    .AnyAsync(x => x.CaseSheetId == model.CaseSheetId
                                   && x.PatientId == model.PatientId
                                   && x.DeptId == model.DeptId
                                   && x.ServiceID == item.ServiceID
                                   && !x.IsCancelled
                                   && !x.IsBilled);

                if (duplicateExists)
                    continue;

                var quantity = item.Quantity <= 0 ? 1 : item.Quantity;
                var rate = service.Cost;
                var discountPer = Math.Clamp(item.DiscountPer, 0, 100);
                var gross = rate * quantity;
                var amount = gross - ((gross * discountPer) / 100m);

                treatments.Add(new PatientTreatment
                {
                    CaseSheetId = model.CaseSheetId,
                    PatientId = model.PatientId,
                    DeptId = model.DeptId,
                    ServiceID = item.ServiceID,
                    DoctorId = model.DoctorId,
                    Quantity = (int)quantity,
                    Rate = rate,
                    DiscountPer = discountPer,
                    Amount = Math.Round(amount, 2),
                    IsBilled = false,
                    IsCancelled = false,
                    CreatedBy = 1,
                    CreatedDate = now,
                    TreatmentDate = now
                });
            }

            if (!treatments.Any())
                return;

            var startedTransaction = _context.Database.CurrentTransaction == null;
            var tx = startedTransaction ? await _context.Database.BeginTransactionAsync() : null;

            try
            {
                _context.PatientTreatments.AddRange(treatments);
                await _context.SaveChangesAsync();

                var treatmentIds = treatments.Select(x => x.PatientTreatmentId).ToList();
                var queuedTreatmentIds = await _context.BillQueueDetails
                    .Where(x => treatmentIds.Contains(x.PatientTreatmentId) && !x.IsCancelled)
                    .Select(x => x.PatientTreatmentId)
                    .ToListAsync();

                var queues = treatments
                    .Where(t => !queuedTreatmentIds.Contains(t.PatientTreatmentId))
                    .Select(t => new BillQueueDetails
                {
                    CaseSheetId = model.CaseSheetId,
                    PatientId = model.PatientId,
                    PatientTreatmentId = t.PatientTreatmentId,
                    DeptId = model.DeptId,
                    ServiceID = t.ServiceID,
                    Quantity = t.Quantity,
                    Amount = t.Amount,
                    IsProcessed = false,
                    IsCancelled = false,
                    CreatedBy = 1,
                    CreatedDate = now
                }).ToList();

                _context.BillQueueDetails.AddRange(queues);
                await _context.SaveChangesAsync();

                if (startedTransaction && tx != null)
                {
                    await tx.CommitAsync();
                }
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

        public async Task<List<PatientTreatment>> GetPatientTreatments(int caseSheetId)
        {
            return await _context.PatientTreatments
                .Include(x => x.Service)
                .Where(x => x.CaseSheetId == caseSheetId && !x.IsCancelled)
                .OrderBy(x => x.TreatmentDate)
                .AsNoTracking()
                .ToListAsync();
        }

        private static List<PatientTreatmentVMItem> ResolveItems(PatientTreatmentVM model)
        {
            var selectedFromList = model.Services?
                .Where(x => x.Selected)
                .Select(x => new PatientTreatmentVMItem
                {
                    TreatmentId = 0,
                    ServiceID = x.ServiceID,
                    ServiceName = x.ServiceName ?? string.Empty,
                    Rate = x.Rate,
                    Quantity = x.Quantity,
                    DiscountPer = x.DiscountPer,
                    Amount = x.Amount
                })
                .ToList() ?? new List<PatientTreatmentVMItem>();

            if (selectedFromList.Any())
                return selectedFromList;

            if (string.IsNullOrWhiteSpace(model.PatientTreatmentJson))
                return new List<PatientTreatmentVMItem>();

            try
            {
                return JsonSerializer.Deserialize<List<PatientTreatmentVMItem>>(
                           model.PatientTreatmentJson,
                           new JsonSerializerOptions
                           {
                               PropertyNameCaseInsensitive = true
                           })
                       ?? new List<PatientTreatmentVMItem>();
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException("Invalid treatment payload.", ex);
            }
        }
    }
}
