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
        private readonly IPatientTreatmentService _patientTreatmentService;
        public GeneralMedicineServices(HmsDbContext db, IMedicationService medicationService, IFollowUpService followUpService, ILookupService lookupService, IReferralStatusService referralService, IPatientTreatmentService patientTreatmentService)
        {
            _db = db;
            _medicationService = medicationService;
            _followUpService = followUpService;
            _lookupService = lookupService;
            _referralService = referralService;
            _patientTreatmentService = patientTreatmentService;
        }

        public async Task<List<TreatmentPatientVm>> GetPendingTreatmentPatients()
        {
            return await _db.Set<TreatmentPatientVm>()
                .FromSqlRaw("EXEC usp_GetPendingTreatmentPatients")
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<GMCasesheetScreenVm> GetTreatmentScreenAsync(int DeptId, int patientId)
        {
            var patient = await _lookupService.GetCaseSheetPatient(patientId);
            var medications = await _medicationService.GetActiveMedications();
            var context = await _lookupService.GetLatestTreatmentContextAsync(DeptId, patientId);
            var currentDept = await _followUpService.GetCurrentDepartmentIdAsync(patientId);
            var departmentServices = await _patientTreatmentService.GetDepartmentServices(DeptId);
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
                Departments = await _lookupService.GetDepartmentsAsync(),
                PatientTreatment = new PatientTreatmentVM
                {
                    CaseSheetId = 0,
                    PatientId = patient.PatientId,
                    DeptId = DeptId,
                    DoctorId = context.DoctorId,
                    Services = departmentServices.Select(x => new PatientServiceVM
                    {
                        ServiceID = x.ServiceID,
                        ServiceName = x.ServiceName,
                        Rate = x.Cost,
                        Quantity = 1,
                        DiscountPer = 0,
                        Selected = false
                    }).ToList()
                }
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

            var deptId = (int)Department.GEN;
            var medications = await _medicationService.GetActiveMedications();
            var existingMeds = await _medicationService.GetPatientMedications(model.PatientId);
            var followUp = await _followUpService.GetLatestFollowUpAsync(model.PatientId);
            var currentDeptId = await _followUpService.GetCurrentDepartmentIdAsync(model.PatientId);
            var departmentServices = await _patientTreatmentService.GetDepartmentServices(deptId);
            var treatments = await _patientTreatmentService.GetPatientTreatments(model.GMID);

            var departments = await _lookupService.GetDepartmentsAsync();
            var referrals = await _referralService.GetReferralsByPatientAsync(model.PatientId);
            var doctors = await _lookupService.GetDoctorsAsync();
            var students = await _lookupService.GetStudentsAsync();

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

                HistoryOfPresentIllness = model.HistoryOfPresentIllness ?? string.Empty,
                PastMedicalHistory = model.PastMedicalHistory ?? string.Empty,
                PastSurgicalHistory = model.PastSurgicalHistory ?? string.Empty,
                FamilyHistory = model.FamilyHistory ?? string.Empty,
                PersonalHistory = model.PersonalHistory ?? string.Empty,
                BloodPressure = model.BloodPressure ?? string.Empty,
                PulseRate = model.PulseRate,
                Temperature = model.Temperature,
                RespiratoryRate = model.RespiratoryRate,
                SpO2 = model.SpO2,
                Weight = model.Weight,
                Height = model.Height,
                BMI = model.BMI,
                GeneralExamination = model.GeneralExamination ?? string.Empty,
                CVSExamination = model.CVSExamination ?? string.Empty,
                RSExamination = model.RSExamination ?? string.Empty,
                AbdomenExamination = model.AbdomenExamination ?? string.Empty,
                CNSExamination = model.CNSExamination ?? string.Empty,


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
                Doctors = doctors,
                Students = students,
                Departments = departments,
                PatientTreatment = new PatientTreatmentVM
                {
                    CaseSheetId = model.GMID,
                    PatientId = model.PatientId,
                    DeptId = deptId,
                    DoctorId = model.DoctorId,
                    Services = departmentServices.Select(x => new PatientServiceVM
                    {
                        ServiceID = x.ServiceID,
                        ServiceName = x.ServiceName,
                        Rate = x.Cost,
                        Quantity = 1,
                        DiscountPer = 0,
                        Selected = false
                    }).ToList(),
                    ExistingTreatments = treatments.Select(x => new PatientTreatmentVMItem
                    {
                        TreatmentId = x.PatientTreatmentId,
                        ServiceID = x.ServiceID,
                        ServiceName = x.Service?.ServiceName ?? string.Empty,
                        Rate = x.Rate,
                        Quantity = x.Quantity,
                        DiscountPer = x.DiscountPer,
                        Amount = x.Amount
                    }).ToList()
                },


                FollowUpSaveVm = new FollowUpSaveVm
                {
                    PatientId = model.PatientId,

                    FollowupDate = followUp?.FollowupDate,
                    FollowupTime = followUp?.FollowupTime,

                    DeptId = followUp?.DeptId ?? 0,

                    DoctorId = followUp?.DoctorId,
                    StudentId = followUp?.StudentId,

                    Status = followUp?.Status ?? "Yet to visit",

                    ReferredTreatmentId = model.ReferredId ?? 0,

                    NextVisitDate = followUp?.FollowupDate,
                    NextVisitTime = followUp?.FollowupTime,

                    NextVisitDepartmentId = followUp?.DeptId,
                    NextVisitDoctorId = followUp?.DoctorId,
                    NextVisitStudentId = followUp?.StudentId,

                    NextVisitReason = followUp?.FollowupReason,

                    Doctors = doctors,
                    Students = students,
                    Departments = departments
                },

                //Doctors = await _lookupService.GetDoctorsAsync(),
                //Students = await _lookupService.GetStudentsAsync(),
                //Departments = await _lookupService.GetDepartmentsAsync(),
                ReferralStatus = new ReferralStatusVm
                {
                    PatientId = model.PatientId,
                    FromDeptId = (int)currentDeptId,
                    Departments = departments,
                    ExistingReferrals = referrals
                },
                ExistingTreatments = treatments
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

            try
            {
                Console.WriteLine("STEP 1: Transaction started");

                GMCasesheet? entity;

                if (isUpdate)
                {
                    entity = await _db.GMCasesheets.FirstOrDefaultAsync(x => x.GMID == model.GMID);

                    if (entity == null)
                    {
                        Console.WriteLine("ERROR: Entity not found for update");
                        throw new Exception("CaseSheet not found");
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

                Console.WriteLine("STEP 2: Entity created/loaded");

                decimal? SafeDecimal(decimal? val, decimal min, decimal max, string field)
                {
                    if (!val.HasValue) return null;

                    if (val < min || val > max)
                    {
                        Console.WriteLine($"WARNING: {field} out of range -> {val}");
                        return null; // prevent SQL overflow
                    }

                    return Math.Round(val.Value, 2);
                }

                int? SafeInt(int? val, int min, int max, string field)
                {
                    if (!val.HasValue) return null;

                    if (val < min || val > max)
                    {
                        Console.WriteLine($"WARNING: {field} out of range -> {val}");
                        return null;
                    }

                    return val;
                }

                entity.PatientId = model.PatientId;
                entity.DoctorId = model.DoctorId;
                entity.StudentId = model.StudentId;
                entity.AllotId = model.AllotId;
                entity.ReferredId = model.ReferredId;
                entity.OpNo = model.OpNo;
                entity.CaseDate = isUpdate ? entity.CaseDate : now;

                Console.WriteLine("STEP 3: Basic info mapped");

                entity.HistoryOfPresentIllness = model.HistoryOfPresentIllness;
                entity.PastMedicalHistory = model.PastMedicalHistory;
                entity.PastSurgicalHistory = model.PastSurgicalHistory;
                entity.FamilyHistory = model.FamilyHistory;
                entity.PersonalHistory = model.PersonalHistory;

                Console.WriteLine("STEP 4: History mapped");

                entity.BloodPressure = model.BloodPressure;
                entity.PulseRate = SafeInt(model.PulseRate, 0, 250, "PulseRate");
                entity.RespiratoryRate = SafeInt(model.RespiratoryRate, 0, 80, "RR");
                entity.SpO2 = SafeInt(model.SpO2, 0, 100, "SpO2");

                entity.Temperature = SafeDecimal(model.Temperature, 20, 60, "Temperature");
                entity.Weight = SafeDecimal(model.Weight, 0, 300, "Weight");
                entity.Height = SafeDecimal(model.Height, 0, 250, "Height");


                Console.WriteLine("STEP 5: Vitals mapped");


                entity.GeneralExamination = model.GeneralExamination;
                entity.CVSExamination = model.CVSExamination;
                entity.RSExamination = model.RSExamination;
                entity.AbdomenExamination = model.AbdomenExamination;
                entity.CNSExamination = model.CNSExamination;

                Console.WriteLine("STEP 6: Examination mapped");

                entity.ChiefComplaint = model.ChiefComplaint;
                entity.Symptoms = model.Symptoms;
                entity.Diagnosis = model.Diagnosis;
                entity.Prescription = model.Prescription;
                entity.Notes = model.Notes;

                Console.WriteLine("STEP 7: Core data mapped");

                entity.Approval1Status = false;
                entity.Approval2Status = false;

                if (isUpdate)
                {
                    entity.ModifiedDate = now;
                    entity.ModifiedBy = 1;
                    entity.ModifiedSystem = Environment.MachineName;
                }

                Console.WriteLine("STEP 8: Before SaveChanges (ENTITY READY)");

                await _db.SaveChangesAsync();
                Console.WriteLine("STEP 9: Main Save SUCCESS");
                await _medicationService.SavePatientMedications(model.PatientId, model.Medications);
                Console.WriteLine("STEP 10: Medications saved");

                //await _followUpService.SaveOrUpdateFollowUpAsync(model);

                var followUpModel = new FollowUpSaveVm
                {
                    PatientId = model.PatientId,

                    FollowupDate = model.NextVisitDate,

                    FollowupTime = model.NextVisitTime,

                    DeptId = model.NextVisitDepartmentId ?? 0,

                    FollowupReason = model.NextVisitReason ?? model.FollowUpNotes,

                    DoctorId = model.NextVisitDoctorId ?? model.DoctorId,

                    StudentId = model.NextVisitStudentId ?? model.StudentId,

                    Status = model.Status ?? "Yet to visit",

                    ReferredTreatmentId = model.ReferredId ?? 0
                };


                await _followUpService.SaveOrUpdateFollowUpAsync(followUpModel);

                Console.WriteLine("STEP 11: FollowUp saved");

                if (model.PatientTreatment != null &&
                    (model.PatientTreatment.Services.Any(x => x.Selected) ||
                     !string.IsNullOrWhiteSpace(model.PatientTreatment.PatientTreatmentJson)))
                {
                    model.PatientTreatment.CaseSheetId = entity.GMID;
                    model.PatientTreatment.PatientId = model.PatientId;
                    model.PatientTreatment.DeptId = (int)Department.GEN;
                    model.PatientTreatment.DoctorId = model.DoctorId;

                    await _patientTreatmentService.SavePatientTreatments(model.PatientTreatment);
                    Console.WriteLine("STEP 11.1: Treatments saved");
                }

                await UpdateAllotmentAndReferralAsync(model, entity.GMID);
                Console.WriteLine("STEP 12: Allotment updated");

                await _db.SaveChangesAsync();
                Console.WriteLine("STEP 13: Final Save SUCCESS");

                await tx.CommitAsync();
                Console.WriteLine("STEP 14: Transaction COMMITTED");
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ ERROR OCCURRED:");
                Console.WriteLine(ex.ToString());

                await tx.RollbackAsync();

                throw; 
            }
        }
     
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
                await _referralService.CompleteReferralAsync(model.ReferredId.Value);
            }

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
     
        public async Task<string> ProcessApprovalFlow(int gmId)
        {
            var gm = await _db.GMCasesheets
                .FirstOrDefaultAsync(x => x.GMID == gmId);

            if (gm == null) return "Case sheet not found.";

            if (gm.IsSentForApproval1 != true)
            {
                gm.IsSentForApproval1 = true;
                gm.ModifiedDate = DateTime.Now;

                await _db.SaveChangesAsync();
                return "Case sheet sent for Approval 1 successfully.";
            }

            if (gm.IsSentForApproval1 == true && gm.Approval1Status != true && gm.IsSentForApproval2 != true)
            {
                gm.Approval1Status = true;
                gm.ModifiedDate = DateTime.Now;

                await _db.SaveChangesAsync();
                return "Approval 1 completed successfully.";
            }

            if (gm.IsSentForApproval1 == true && gm.Approval1Status == true && gm.IsSentForApproval2 != true)
            {
                gm.IsSentForApproval2 = true;
                gm.ModifiedDate = DateTime.Now;

                await _db.SaveChangesAsync();
                return "Case sheet sent for Approval 2 successfully.";
            }

            if (gm.IsSentForApproval1 == true && gm.Approval1Status == true && gm.IsSentForApproval2 == true && gm.Approval2Status != true)
            {
                gm.Approval2Status = true;
                gm.ModifiedDate = DateTime.Now;

                await _db.SaveChangesAsync();
                return "Approval 2 completed successfully.";
            }

            return "No action performed.";
        }



        public async Task<List<GMApprovalQueueVm>> GetApprovalQueue(DateTime fromDate, DateTime toDate)
        {
            var parameters = new[]
            {
                new SqlParameter("@FromDate", fromDate),
                new SqlParameter("@ToDate", toDate)
            };

            return await _db.Set<GMApprovalQueueVm>().FromSqlRaw("EXEC usp_GetGMApprovalQueue @FromDate,@ToDate", parameters).AsNoTracking().ToListAsync();
        }      
    }
}
