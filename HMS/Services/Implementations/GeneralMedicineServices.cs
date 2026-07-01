using HMS.Data;
using HMS.Entities;
using HMS.Models;
using HMS.Services.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace HMS.Services.Implementations
{
    public class GeneralMedicineServices : IGeneralMedicineServices
    {
        private readonly HmsDbContext _db;

        public GeneralMedicineServices(HmsDbContext db)
        {
            _db = db;
        }

        public async Task<List<TreatmentPatientVm>> GetPendingTreatmentPatients()
        {
            return await _db.Set<TreatmentPatientVm>()
                .FromSqlRaw("EXEC usp_GetPendingTreatmentPatients")
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<GMCasesheetViewVm> GetCaseSheetPatient(int patientId)
        {
            var param = new SqlParameter("@PatientId", patientId);

            var data = await _db.Set<GMCasesheetViewVm>()
                .FromSqlRaw("EXEC usp_GetGMCaseSheetPatient @PatientId", param)
                .AsNoTracking()
                .ToListAsync();

            return data.FirstOrDefault() ?? new GMCasesheetViewVm();
        }

        public async Task<List<MASMedication>> GetActiveMedications()
        {
            return await _db.MASMedications
                .AsNoTracking()
                .Where(x => x.IsActive && !x.DelInd)
                .OrderBy(x => x.Medication)
                .ToListAsync();
        }

        public async Task<GMCasesheetScreenVm> GetTreatmentScreenAsync(int patientId)
        {
            var patient = await GetCaseSheetPatient(patientId);
            var medications = await GetActiveMedications();

            return new GMCasesheetScreenVm
            {
                PatientId = patient.PatientId,
                OpNo = patient.OpNo ?? string.Empty,
                PatientName = patient.PatientName ?? string.Empty,
                Age = patient.Age,
                Gender = patient.Gender ?? string.Empty,
                DoctorId = patient.DoctorId,
                DoctorName = patient.DoctorName ?? string.Empty,
                StudentId = patient.StudentId,
                StudentName = patient.StudentName ?? string.Empty,
                AllotId = patient.AllotId,
                ReferredId = patient.ReferredId,
                MedicationMaster = medications,
                Medications = new List<PatientMedicationVm> { new() }
            };
        }

        private async Task<List<PatientMedicationVm>> GetPatientMedications(int patientId)
        {
            return await (
                from detail in _db.PatientMedicationDetails.AsNoTracking()
                join med in _db.MASMedications.AsNoTracking()
                    on detail.MedicationId equals med.MedId
                where detail.PatientId == patientId && detail.IsActive && med.IsActive && !med.DelInd
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

        public async Task<GMCasesheetScreenVm> GetCaseSheetById(int gmId)
        {
            var param = new SqlParameter("@GMID", gmId);

            var model = _db.Set<GMCasesheetSaveVm>()
                .FromSqlRaw("EXEC usp_GetCaseSheetById @GMID", param)
                .AsNoTracking()
                .AsEnumerable()
                .FirstOrDefault();

            if (model == null)
                return new GMCasesheetScreenVm();

            var medications = await GetActiveMedications();
            var existingMeds = await GetPatientMedications(model.PatientId);

            return new GMCasesheetScreenVm
            {
                GMID = model.GMID,
                PatientId = model.PatientId,
                OpNo = model.OpNo,
                PatientName = model.PatientName,
                Age = model.Age,
                Gender = model.Gender,
                DoctorId = model.DoctorId,
                DoctorName = model.DoctorName,
                StudentId = model.StudentId,
                StudentName = model.StudentName,
                AllotId = model.AllotId,
                ReferredId = model.ReferredId,
                ChiefComplaint = model.ChiefComplaint,
                Symptoms = model.Symptoms,
                Diagnosis = model.Diagnosis,
                Prescription = model.Prescription,
                Notes = model.Notes,
                MedicationMaster = medications,
                Medications = existingMeds.Any()
                    ? existingMeds
                    : new List<PatientMedicationVm> { new() }
            };
        }

        public async Task SaveCaseSheet(GMCasesheetSaveVm model)
        {
            await using var tx = await _db.Database.BeginTransactionAsync();

            var entity = new GMCasesheet
            {
                PatientId = model.PatientId,
                DoctorId = model.DoctorId,
                StudentId = model.StudentId,
                AllotId = model.AllotId,
                ReferredId = model.ReferredId,
                OpNo = model.OpNo,
                CaseDate = DateTime.Now,
                ChiefComplaint = model.ChiefComplaint,
                Symptoms = model.Symptoms,
                Diagnosis = model.Diagnosis,
                Prescription = model.Prescription,
                Notes = model.Notes,
                CreatedDate = DateTime.Now,
                CreatedBy = 1
            };

            _db.GMCasesheets.Add(entity);
            await _db.SaveChangesAsync();

            if (model.Medications.Any())
            {
                foreach (var med in model.Medications.Where(x => x.MedicationId > 0))
                {
                    _db.PatientMedicationDetails.Add(new PatientMedicationDetails
                    {
                        PatientId = model.PatientId,
                        MedicationId = med.MedicationId,
                        Frequency = med.Frequency,
                        Remarks = med.Remarks,
                        Duration = med.Duration,
                        CreatedDate = DateTime.Now,
                        CreatedBy = 1
                    });
                }

                await _db.SaveChangesAsync();
            }

            if (model.AllotId.HasValue)
            {
                var allotment = await _db.StudentAllotments.FirstOrDefaultAsync(x => x.AllotId == model.AllotId);
                if (allotment != null)
                {
                    allotment.CaserecordId = entity.GMID;
                }
            }

            if (model.ReferredId.HasValue)
            {
                var referral = await _db.ReferralStatuses.FirstOrDefaultAsync(x => x.ReferredId == model.ReferredId);
                if (referral != null)
                {
                    referral.TreatmentStatus = "Completed";
                }
            }

            await _db.SaveChangesAsync();
            await tx.CommitAsync();
        }

        public async Task<List<GMCasesheetSearchVm>> GetCompletedCases()
        {
            return await _db.Set<GMCasesheetSearchVm>()
                .FromSqlRaw("EXEC usp_GetCompletedCasesheets")
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task UpdateCaseSheet(GMCasesheetSaveVm model)
        {
            var entity = await _db.GMCasesheets.FirstOrDefaultAsync(x => x.GMID == model.GMID);
            if (entity == null)
                return;

            entity.ChiefComplaint = model.ChiefComplaint;
            entity.Symptoms = model.Symptoms;
            entity.Diagnosis = model.Diagnosis;
            entity.Prescription = model.Prescription;
            entity.Notes = model.Notes;
            entity.ModifiedDate = DateTime.Now;
            entity.ModifiedBy = 1;

            await _db.PatientMedicationDetails
                .Where(x => x.PatientId == model.PatientId)
                .ExecuteDeleteAsync();

            foreach (var med in model.Medications.Where(x => x.MedicationId > 0))
            {
                _db.PatientMedicationDetails.Add(new PatientMedicationDetails
                {
                    PatientId = model.PatientId,
                    MedicationId = med.MedicationId,
                    Frequency = med.Frequency,
                    Remarks = med.Remarks,
                    Duration = med.Duration,
                    CreatedDate = DateTime.Now,
                    CreatedBy = 1
                });
            }

            await _db.SaveChangesAsync();
        }
    }
}
