using HMS.Data;
using HMS.Entities;
using HMS.Models;
using HMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace HMS.Services.Implementations
{
    public class GeneralMedicineServices : IGeneralMedicineServices
    {
        private readonly HmsDbContext _db;
        private readonly IMedicationService _medicationService;
        private readonly IFollowUpService _followUpService;
        private readonly ILookupService _lookupService;
        public GeneralMedicineServices(HmsDbContext db, IMedicationService medicationService, IFollowUpService followUpService, ILookupService lookupService)
        {
            _db = db;
            _medicationService = medicationService;
            _followUpService = followUpService;
            _lookupService = lookupService;

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
                .FromSqlRaw("EXEC SP_GetPatientDetailsById @PatientId", param)
                .AsNoTracking()
                .ToListAsync();

            var model = data.FirstOrDefault() ?? new GMCasesheetViewVm();
            model.PatientId = patientId;
            return model;
        }

        //public async Task<List<MASMedication>> GetActiveMedications()
        //{
        //    return await _db.MASMedications
        //        .AsNoTracking()
        //        .Where(x => x.IsActive && !x.DelInd)
        //        .OrderBy(x => x.Medication)
        //        .ToListAsync();
        //}

        public async Task<GMCasesheetScreenVm> GetTreatmentScreenAsync(int patientId)
        {
            var patient = await GetCaseSheetPatient(patientId);
            //var medications = await GetActiveMedications();
            var medications = await _medicationService.GetActiveMedications();
            var context = await GetLatestTreatmentContextAsync(patientId);
            var currentDept = await _followUpService.GetCurrentDepartmentIdAsync(patientId);
            return new GMCasesheetScreenVm
            {
                PatientId = patient.PatientId,
                OpNo = patient.OpNo ?? string.Empty,
                PatientName = patient.PatientName ?? string.Empty,
                Age = patient.Age ?? 0,
                Gender = patient.Gender ?? string.Empty,
                DoctorId = context.DoctorId,
                DoctorName = context.DoctorName,
                StudentId = context.StudentId,
                StudentName = context.StudentName,
                AllotId = context.AllotId,
                ReferredId = context.ReferredId,
                //NextVisitDepartmentId = context.DeptId,
                

                NextVisitDepartmentId = currentDept,
                NextVisitDoctorId = context.DoctorId == 0 ? null : context.DoctorId,
                NextVisitStudentId = context.StudentId == 0 ? null : context.StudentId,
                MedicationMaster = medications,
                Medications = new List<PatientMedicationVm> { new() },
                //Doctors = await GetDoctors(),
                //Students = await GetStudents(),
                //Departments = await GetDepartments()
                Doctors = await _lookupService.GetDoctorsAsync(),
                Students = await _lookupService.GetStudentsAsync(),
                Departments = await _lookupService.GetDepartmentsAsync()
            };
        }

        public async Task<GMCasesheetScreenVm> GetCaseSheetById(int gmId)
        {
            var param = new SqlParameter("@GMID", gmId);

            var data = await _db.Set<GMCasesheetDbVm>()
                .FromSqlRaw("EXEC usp_GetCaseSheetById @GMID", param)
                .AsNoTracking()
                .ToListAsync();

            var model = data.FirstOrDefault();

            if (model == null)
            {
                return new GMCasesheetScreenVm();
            }

            //var medications = await GetActiveMedications();
            var medications = await _medicationService.GetActiveMedications();
            //var existingMeds = await GetPatientMedications(model.PatientId);
            var existingMeds = await _medicationService.GetPatientMedications(model.PatientId);
            //var followUp = await GetLatestFollowUpAsync(model.PatientId);
            //var currentDeptId = await GetCurrentDepartmentId(model.PatientId);
            var followUp = await _followUpService.GetLatestFollowUpAsync(model.PatientId);
            var currentDeptId = await _followUpService.GetCurrentDepartmentIdAsync(model.PatientId);

            return new GMCasesheetScreenVm
            {
                GMID = model.GMID,
                PatientId = model.PatientId,
                OpNo = model.OpNo ?? string.Empty,
                PatientName = model.PatientName ?? string.Empty,
                Age = model.Age ?? 0,
                Gender = model.Gender ?? string.Empty,
                DoctorId = model.DoctorId,
                StudentId = model.StudentId,
                AllotId = model.AllotId,
                ReferredId = model.ReferredId,
                DoctorName = model.DoctorName ?? string.Empty,
                StudentName = model.StudentName ?? string.Empty,
                ChiefComplaint = model.ChiefComplaint ?? string.Empty,
                Symptoms = model.Symptoms ?? string.Empty,
                Diagnosis = model.Diagnosis ?? string.Empty,
                Prescription = model.Prescription ?? string.Empty,
                Notes = model.Notes ?? string.Empty,
                MedicationMaster = medications,
                Medications = existingMeds.Any() ? existingMeds : new List<PatientMedicationVm> { new() },
                NextVisitDate = followUp?.FollowupDate,
                NextVisitTime = followUp?.FollowupTime,
                NextVisitDepartmentId = (followUp?.DeptId ?? 0) > 0 ? followUp?.DeptId : currentDeptId,
                NextVisitDoctorId = followUp?.DoctorId,
                NextVisitStudentId = followUp?.StudentId,
                NextVisitReason = followUp?.FollowupReason,
                Status = followUp?.Status ?? "Yet to visit",
                //Doctors = await GetDoctors(),
                //Students = await GetStudents(),
                //Departments = await GetDepartments()
                Doctors = await _lookupService.GetDoctorsAsync(),
                Students = await _lookupService.GetStudentsAsync(),
                Departments = await _lookupService.GetDepartmentsAsync()
            };
        }

        public async Task SaveCaseSheet(GMCasesheetSaveVm model)
        {
            await SaveOrUpdateCaseSheetAsync(model, isUpdate: false);
        }

        //public async Task<List<GMCasesheetSearchVm>> GetCompletedCases()
        //{
        //    return await _db.Set<GMCasesheetSearchVm>()
        //        .FromSqlRaw("EXEC usp_GetCompletedCasesheets")
        //        .AsNoTracking()
        //        .ToListAsync();
        //}

        public async Task<List<GMCasesheetSearchVm>> GetCompletedCases(DateTime fromDate, DateTime toDate)
        {
            var fromParam = new SqlParameter("@FromDate", fromDate.Date);
            var toParam = new SqlParameter("@ToDate", toDate.Date);

            return await _db.Set<GMCasesheetSearchVm>()
                .FromSqlRaw(
                    "EXEC usp_GetCompletedCasesheets @FromDate, @ToDate",
                    fromParam,
                    toParam)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task UpdateCaseSheet(GMCasesheetSaveVm model)
        {
            await SaveOrUpdateCaseSheetAsync(model, isUpdate: true);
        }

        private async Task SaveOrUpdateCaseSheetAsync(GMCasesheetSaveVm model, bool isUpdate)
        {
            await using var tx = await _db.Database.BeginTransactionAsync();
            var now = DateTime.Now;

            GMCasesheet? entity;
            if (isUpdate)
            {
                entity = await _db.GMCasesheets.FirstOrDefaultAsync(x => x.GMID == model.GMID);
                if (entity == null)
                {
                    return;
                }
            }
            else
            {
                entity = new GMCasesheet
                {
                    CreatedDate = now,
                    CreatedBy = 1,
                    CreatedSystem = Environment.MachineName
                };
                _db.GMCasesheets.Add(entity);
            }

            entity.PatientId = model.PatientId;
            entity.DoctorId = model.DoctorId;
            entity.StudentId = model.StudentId;
            entity.AllotId = model.AllotId;
            entity.ReferredId = model.ReferredId;
            entity.OpNo = model.OpNo;
            entity.CaseDate = isUpdate ? entity.CaseDate : now;
            entity.ChiefComplaint = model.ChiefComplaint;
            entity.Symptoms = model.Symptoms;
            entity.Diagnosis = model.Diagnosis;
            entity.Prescription = model.Prescription;
            entity.Notes = model.Notes;

            if (isUpdate)
            {
                entity.ModifiedDate = now;
                entity.ModifiedBy = 1;
                entity.ModifiedSystem = Environment.MachineName;
            }

            await _db.SaveChangesAsync();

            //await ReplacePatientMedicationsAsync(model.PatientId, model.Medications, now);
            await _medicationService.SavePatientMedications(model.PatientId, model.Medications);
            //await SaveOrUpdateFollowUpAsync(model, now);
            await _followUpService.SaveOrUpdateFollowUpAsync(model);
            await UpdateAllotmentAndReferralAsync(model, entity.GMID);

            await _db.SaveChangesAsync();
            await tx.CommitAsync();
        }

        //private async Task ReplacePatientMedicationsAsync(int patientId, IEnumerable<PatientMedicationVm>? medications, DateTime now)
        //{
        //    await _db.PatientMedicationDetails
        //        .Where(x => x.PatientId == patientId)
        //        .ExecuteDeleteAsync();

        //    var validMedications = (medications ?? Enumerable.Empty<PatientMedicationVm>())
        //        .Where(x => x.MedicationId > 0)
        //        .ToList();

        //    if (!validMedications.Any())
        //    {
        //        return;
        //    }

        //    foreach (var med in validMedications)
        //    {
        //        _db.PatientMedicationDetails.Add(new PatientMedicationDetails
        //        {
        //            PatientId = patientId,
        //            MedicationId = med.MedicationId,
        //            Frequency = med.Frequency,
        //            Remarks = med.Remarks,
        //            Duration = med.Duration,
        //            CreatedDate = now,
        //            CreatedBy = 1
        //        });
        //    }
        //}

        //private async Task SaveOrUpdateFollowUpAsync(GMCasesheetSaveVm model, DateTime now)
        //{
        //    if (!model.NextVisitDate.HasValue)
        //    {
        //        return;
        //    }

        //    var deptId = model.NextVisitDepartmentId.GetValueOrDefault();
        //    if (deptId <= 0)
        //    {
        //        deptId = await GetCurrentDepartmentId(model.PatientId) ?? 0;
        //    }

        //    if (deptId <= 0)
        //    {
        //        return;
        //    }

        //    var reason = model.NextVisitReason ?? model.FollowUpNotes;
        //    var doctorId = model.NextVisitDoctorId ?? model.DoctorId;
        //    var studentId = model.NextVisitStudentId ?? model.StudentId;
        //    var latest = await _db.FollowUps
        //        .Where(x => x.PatientId == model.PatientId)
        //        .OrderByDescending(x => x.FollowupId)
        //        .FirstOrDefaultAsync();

        //    if (latest == null)
        //    {
        //        _db.FollowUps.Add(new FollowUp
        //        {
        //            PatientId = model.PatientId,
        //            FollowupDate = model.NextVisitDate.Value,
        //            FollowupTime = model.NextVisitTime,
        //            DeptId = deptId,
        //            FollowupReason = reason,
        //            DoctorId = doctorId,
        //            StudentId = studentId,
        //            Status = model.Status ?? "Yet to visit",
        //            ReferredTreatmentId = model.ReferredId.GetValueOrDefault(),
        //            CreatedDate = now,
        //            CreatedBy = "System",
        //            CreatedSystem = Environment.MachineName,
        //            IsCancelled = false
        //        });
        //        return;
        //    }

        //    latest.FollowupDate = model.NextVisitDate.Value;
        //    latest.FollowupTime = model.NextVisitTime;
        //    latest.DeptId = deptId;
        //    latest.FollowupReason = reason;
        //    latest.DoctorId = doctorId;
        //    latest.StudentId = studentId;
        //    latest.Status = model.Status ?? latest.Status;
        //    latest.ReferredTreatmentId = model.ReferredId.GetValueOrDefault();
        //    latest.ModifiedDate = now;
        //    latest.ModifiedBy = "System";
        //    latest.ModifiedSystem = Environment.MachineName;
        //}

        private async Task UpdateAllotmentAndReferralAsync(GMCasesheetSaveVm model, int gmId)
        {
            if (model.AllotId.HasValue)
            {
                var allotment = await _db.StudentAllotments
                    .FirstOrDefaultAsync(x => x.AllotId == model.AllotId.Value);

                if (allotment != null)
                {
                    allotment.CaserecordId = gmId;
                }
            }

            if (model.ReferredId.HasValue)
            {
                var referral = await _db.ReferralStatuses
                    .FirstOrDefaultAsync(x => x.ReferredId == model.ReferredId.Value);

                if (referral != null)
                {
                    referral.TreatmentStatus = "Completed";
                }
            }
        }

        //private async Task<List<PatientMedicationVm>> GetPatientMedications(int patientId)
        //{
        //    return await (
        //        from detail in _db.PatientMedicationDetails.AsNoTracking()
        //        join med in _db.MASMedications.AsNoTracking()
        //            on detail.MedicationId equals med.MedId
        //        where detail.PatientId == patientId && detail.IsActive && med.IsActive && !med.DelInd
        //        orderby detail.PatientMedicationId
        //        select new PatientMedicationVm
        //        {
        //            MedicationId = detail.MedicationId,
        //            MedicationName = med.Medication,
        //            Frequency = detail.Frequency,
        //            Remarks = detail.Remarks,
        //            Duration = detail.Duration
        //        }).ToListAsync();
        //}

        //private async Task<FollowUp?> GetLatestFollowUpAsync(int patientId)
        //{
        //    return await _db.FollowUps
        //        .AsNoTracking()
        //        .Where(x => x.PatientId == patientId)
        //        .OrderByDescending(x => x.FollowupId)
        //        .FirstOrDefaultAsync();
        //}

        //private async Task<int?> GetCurrentDepartmentId(int patientId)
        //{
        //    return await _db.StudentAllotments
        //        .AsNoTracking()
        //        .Where(x => x.PatientId == patientId && x.DeptId.HasValue)
        //        .OrderByDescending(x => x.AllotId)
        //        .Select(x => x.DeptId)
        //        .FirstOrDefaultAsync();
        //}

        private async Task<TreatmentContextVm> GetLatestTreatmentContextAsync(int patientId)
        {
            var allotment = await _db.StudentAllotments
                .AsNoTracking()
                .Where(x => x.PatientId == patientId)
                .OrderByDescending(x => x.AllotId)
                .FirstOrDefaultAsync();

            var referral = await _db.ReferralStatuses
                .AsNoTracking()
                .Where(x => x.PatientId == patientId)
                .OrderByDescending(x => x.ReferredId)
                .FirstOrDefaultAsync();

            var doctorName = string.Empty;
            if (allotment?.DoctorId.HasValue == true)
            {
                doctorName = await _db.Doctors
                    .AsNoTracking()
                    .Where(x => x.DoctorId == allotment.DoctorId.Value)
                    .Select(x => x.DoctorName)
                    .FirstOrDefaultAsync() ?? string.Empty;
            }

            var studentName = string.Empty;
            if (allotment?.StudentId.HasValue == true)
            {
                studentName = await _db.Students
                    .AsNoTracking()
                    .Where(x => x.StudentId == allotment.StudentId.Value)
                    .Select(x => x.StudentName)
                    .FirstOrDefaultAsync() ?? string.Empty;
            }

            return new TreatmentContextVm
            {
                AllotId = allotment?.AllotId,
                ReferredId = referral?.ReferredId,
                DeptId = allotment?.DeptId,
                DoctorId = allotment?.DoctorId ?? 0,
                DoctorName = doctorName,
                StudentId = allotment?.StudentId ?? 0,
                StudentName = studentName
            };
        }

        //private async Task<List<SelectListItem>> GetDoctors()
        //{
        //    return await _db.Doctors
        //        .AsNoTracking()
        //        .Where(x => x.IsActive)
        //        .OrderBy(x => x.DoctorName)
        //        .Select(x => new SelectListItem
        //        {
        //            Value = x.DoctorId.ToString(),
        //            Text = x.DoctorName
        //        })
        //        .ToListAsync();
        //}

        //private async Task<List<SelectListItem>> GetStudents()
        //{
        //    return await _db.Students
        //        .AsNoTracking()
        //        .Where(x => x.IsActive)
        //        .OrderBy(x => x.StudentName)
        //        .Select(x => new SelectListItem
        //        {
        //            Value = x.StudentId.ToString(),
        //            Text = x.StudentName
        //        })
        //        .ToListAsync();
        //}

        //private async Task<List<SelectListItem>> GetDepartments()
        //{
        //    return await _db.MASDepartments
        //        .AsNoTracking()
        //        .Where(x => x.IsActive)
        //        .OrderBy(x => x.DeptName)
        //        .Select(x => new SelectListItem
        //        {
        //            Value = x.DeptId.ToString(),
        //            Text = x.DeptName
        //        })
        //        .ToListAsync();
        //}

        private sealed class TreatmentContextVm
        {
            public int? AllotId { get; set; }
            public int? ReferredId { get; set; }
            public int? DeptId { get; set; }
            public int DoctorId { get; set; }
            public string DoctorName { get; set; } = string.Empty;
            public int StudentId { get; set; }
            public string StudentName { get; set; } = string.Empty;
        }
    }
}
