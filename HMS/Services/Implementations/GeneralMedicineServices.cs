using HMS.Common;
using HMS.Data;
using HMS.Entities;
using HMS.Models;
using HMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace HMS.Services.Implementations
{
    public class GeneralMedicineServices : IGeneralMedicineServices
    {
        private readonly HmsDbContext _db;
        private readonly IMedicationService _medicationService;
        private readonly IFollowUpService _followUpService;
        private readonly ILookupService _lookupService;
        private readonly IReferralStatusService _referralService;
        public GeneralMedicineServices(HmsDbContext db, IMedicationService medicationService, IFollowUpService followUpService, ILookupService lookupService, IReferralStatusService referralService)
        {
            _db = db;
            _medicationService = medicationService;
            _followUpService = followUpService;
            _lookupService = lookupService;
            _referralService = referralService;
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

        public async Task<GMCasesheetScreenVm> GetTreatmentScreenAsync(int patientId)
        {
            var patient = await GetCaseSheetPatient(patientId);
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
                NextVisitDepartmentId = currentDept,
                NextVisitDoctorId = context.DoctorId == 0 ? null : context.DoctorId,
                NextVisitStudentId = context.StudentId == 0 ? null : context.StudentId,
                MedicationMaster = medications,
                Medications = new List<PatientMedicationVm> { new() },
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

            var medications = await _medicationService.GetActiveMedications();
            var existingMeds = await _medicationService.GetPatientMedications(model.PatientId);
            var followUp = await _followUpService.GetLatestFollowUpAsync(model.PatientId);
            var currentDeptId = await _followUpService.GetCurrentDepartmentIdAsync(model.PatientId);

            var departments = await _lookupService.GetDepartmentsAsync();
            var referrals = await _referralService.GetReferralsByPatientAsync(model.PatientId);

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
                IsSentForApproval1 = model.IsSentForApproval1,
                IsSentForApproval2 = model.IsSentForApproval2,
                Approval1Status = model.Approval1Status,
                Approval2Status = model.Approval2Status,
                MedicationMaster = medications,
                Medications = existingMeds.Any() ? existingMeds : new List<PatientMedicationVm> { new() },
                NextVisitDate = followUp?.FollowupDate,
                NextVisitTime = followUp?.FollowupTime,
                NextVisitDepartmentId = (followUp?.DeptId ?? 0) > 0 ? followUp?.DeptId : currentDeptId,
                NextVisitDoctorId = followUp?.DoctorId,
                NextVisitStudentId = followUp?.StudentId,
                NextVisitReason = followUp?.FollowupReason,
                Status = followUp?.Status ?? "Yet to visit",
                Doctors = await _lookupService.GetDoctorsAsync(),
                Students = await _lookupService.GetStudentsAsync(),
                Departments = await _lookupService.GetDepartmentsAsync(),
                ReferralStatus = new ReferralStatusVm
                {
                    PatientId = model.PatientId,
                    FromDeptId = (int)currentDeptId,
                    Departments = departments,
                    ExistingReferrals = referrals
                }
            };
        }

        public async Task SaveCaseSheet(GMCasesheetSaveVm model)
        {
            await SaveOrUpdateCaseSheetAsync(model, isUpdate: false);
        }



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
            entity.Approval1Status = false;
            entity.Approval2Status = false;


            if (isUpdate)
            {
                entity.ModifiedDate = now;
                entity.ModifiedBy = 1;
                entity.ModifiedSystem = Environment.MachineName;
            }

            await _db.SaveChangesAsync();

            await _medicationService.SavePatientMedications(model.PatientId, model.Medications);

            await _followUpService.SaveOrUpdateFollowUpAsync(model);
            await UpdateAllotmentAndReferralAsync(model, entity.GMID);

            await _db.SaveChangesAsync();
            await tx.CommitAsync();
        }


        private async Task UpdateAllotmentAndReferralAsync(GMCasesheetSaveVm model, int gmId)
        {
            // 1. Update allotment
            if (model.AllotId.HasValue)
            {
                var allotment = await _db.StudentAllotments
                    .FirstOrDefaultAsync(x => x.AllotId == model.AllotId.Value);

                if (allotment != null)
                {
                    allotment.CaserecordId = gmId;
                }
            }

            // 2. Complete the referral that brought the patient here
            if (model.ReferredId.HasValue)
            {
                await _referralService.CompleteReferralAsync(model.ReferredId.Value);
            }

            // 3. Create new referrals only if any departments were selected
            if (model.SelectedToDeptIds != null && model.SelectedToDeptIds.Any())
            {
                var fromDeptId = await _followUpService.GetCurrentDepartmentIdAsync(model.PatientId);

                if (fromDeptId.HasValue)
                {
                    await _referralService.CreateReferralsAsync(
                        model.PatientId,
                        fromDeptId.Value,
                        model.SelectedToDeptIds,
                        model.Reasons ?? new Dictionary<int, string>());
                }
            }
        }

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

        public async Task ProcessApprovalFlow(int gmId)
        {
            var gm = await _db.GMCasesheets
                .FirstOrDefaultAsync(x => x.GMID == gmId);

            if (gm == null)
                return;

            if ((bool)!gm.IsSentForApproval1)
            {
                gm.IsSentForApproval1 = true;
                gm.ModifiedDate = DateTime.Now;
            }

            else if (gm.IsSentForApproval1 == true
                  && gm.Approval1Status != true
                  && gm.IsSentForApproval2 == false)
            {
                gm.Approval1Status = true;
                gm.ModifiedDate = DateTime.Now;
            }

            else if (gm.IsSentForApproval1 == true
                  && gm.Approval1Status == true
                  && gm.IsSentForApproval2 == false)
            {
                gm.IsSentForApproval2 = true;
                gm.ModifiedDate = DateTime.Now;
            }

            else if (gm.IsSentForApproval1 == true
                  && gm.Approval1Status == true
                  && gm.IsSentForApproval2 == true
                  && gm.Approval2Status != true)
            {
                gm.Approval2Status = true;
                gm.ModifiedDate = DateTime.Now;
            }

            await _db.SaveChangesAsync();
        }



        public async Task<List<GMApprovalQueueVm>> GetApprovalQueue(DateTime fromDate, DateTime toDate)
        {
            var parameters = new[]
            {
                new SqlParameter("@FromDate", fromDate),
                new SqlParameter("@ToDate", toDate)
            };

            return await _db.Set<GMApprovalQueueVm>()
                .FromSqlRaw("EXEC usp_GetGMApprovalQueue @FromDate,@ToDate", parameters)
                .AsNoTracking()
                .ToListAsync();
        }
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
