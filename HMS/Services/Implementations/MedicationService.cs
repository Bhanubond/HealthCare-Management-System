using HMS.Data;
using HMS.Entities;
using HMS.Models;
using HMS.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HMS.Services.Implementations
{
    public class MedicationService : IMedicationService
    {
        private readonly HmsDbContext _db;

        public MedicationService(HmsDbContext db)
        {
            _db = db;
        }

        public async Task<List<MASMedication>> GetActiveMedications()
        {
            return await _db.MASMedications
                .AsNoTracking()
                .Where(x => x.IsActive && !x.DelInd)
                .OrderBy(x => x.Medication)
                .ToListAsync();
        }

        public async Task<List<PatientMedicationVm>> GetPatientMedications(int patientId)
        {
            return await (
                from detail in _db.PatientMedicationDetails.AsNoTracking()
                join med in _db.MASMedications.AsNoTracking()
                    on detail.MedicationId equals med.MedId
                where detail.PatientId == patientId
                      && detail.IsActive
                      && med.IsActive
                      && !med.DelInd
                orderby detail.PatientMedicationId
                select new PatientMedicationVm
                {
                    MedicationId = detail.MedicationId,
                    MedicationName = med.Medication,
                    Frequency = detail.Frequency,
                    Remarks = detail.Remarks,
                    Duration = detail.Duration
                }).ToListAsync();
        }

        public async Task SavePatientMedications(
            int patientId,
            IEnumerable<PatientMedicationVm>? medications)
        {
            // Remove existing medications
            await _db.PatientMedicationDetails
                .Where(x => x.PatientId == patientId)
                .ExecuteDeleteAsync();

            var validMedications = (medications ?? Enumerable.Empty<PatientMedicationVm>())
                .Where(x => x.MedicationId > 0)
                .ToList();

            if (!validMedications.Any())
                return;

            var now = DateTime.Now;

            foreach (var med in validMedications)
            {
                _db.PatientMedicationDetails.Add(new PatientMedicationDetails
                {
                    PatientId = patientId,
                    MedicationId = med.MedicationId,
                    Frequency = med.Frequency,
                    Remarks = med.Remarks,
                    Duration = med.Duration,
                    CreatedDate = now,
                    CreatedBy = 1,
                    IsActive = true
                });
            }

            //await _db.SaveChangesAsync();
        }
    }
}