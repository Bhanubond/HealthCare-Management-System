using HMS.Common;
using HMS.Data;
using HMS.Entities;
using HMS.Models;
using HMS.Services.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace HMS.Services.Implementations
{
    public class EmergencyService : IEmergencyService
    {
        private readonly HmsDbContext _db;
        private readonly ILookupService _lookupService;
        private readonly IMedicationService _medicationService;
        private readonly IFollowUpService _followUpService;
        private readonly IReferralStatusService _referralService;
        private readonly IPatientTreatmentService _patientTreatmentService;

        public EmergencyService(HmsDbContext db, ILookupService lookupService, IMedicationService medicationService, IReferralStatusService referralService, IFollowUpService followUpService, IPatientTreatmentService patientTreatmentService)
        {
            _db = db;
            _lookupService = lookupService;
            _medicationService = medicationService;
            _referralService = referralService;
            _followUpService = followUpService;
            _patientTreatmentService = patientTreatmentService;
        }
        public async Task<EMRCasesheetScreenVm> GetTreatmentScreenAsync(int DeptId, int patientId)
        {
            var patient = await _lookupService.GetCaseSheetPatient(patientId);
            var medications = await _medicationService.GetActiveMedications();
            var context = await _lookupService.GetLatestTreatmentContextAsync(DeptId, patientId);
            var currentDept = await _followUpService.GetCurrentDepartmentIdAsync(patientId);
            return new EMRCasesheetScreenVm
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


        public async Task SaveCaseSheet(EMRCasesheetScreenVm model)
        {
            await SaveOrUpdateCaseSheetAsync(model, isUpdate: false);
        }
        public async Task UpdateCaseSheet(EMRCasesheetScreenVm model)
        {
            await SaveOrUpdateCaseSheetAsync(model, isUpdate: true);
        }

        private async Task SaveOrUpdateCaseSheetAsync(EMRCasesheetScreenVm model, bool isUpdate)
        {
            await using var tx = await _db.Database.BeginTransactionAsync();
            var now = DateTime.Now;

            try
            {
                Console.WriteLine("STEP 1: Transaction started");

                EMRCasesheet? entity;

                if (isUpdate)
                {
                    entity = await _db.EMRCasesheets.FirstOrDefaultAsync(x => x.EMRId == model.EMRId);

                    if (entity == null)
                    {
                        Console.WriteLine("ERROR: Entity not found for update");
                        throw new Exception("CaseSheet not found");
                    }
                }
                else
                {
                    entity = new EMRCasesheet
                    {
                        CreatedDate = now,
                        CreatedBy = 1,
                        CreatedSystem = Environment.MachineName
                    };

                    _db.EMRCasesheets.Add(entity);
                }

                Console.WriteLine("STEP 2: Entity created/loaded");

                decimal? SafeDecimal(decimal? val, decimal min, decimal max, string field)
                {
                    if (!val.HasValue) return null;

                    if (val < min || val > max)
                    {
                        Console.WriteLine($"WARNING: {field} out of range -> {val}");
                        return null; 
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


                entity.ArrivalMode = model.ArrivalMode;
                entity.ArrivalTime = model.ArrivalTime;
                entity.BroughtBy = model.BroughtBy;


                entity.TriageCategory = model.TriageCategory;
                entity.TriageTime = model.TriageTime;
                entity.TriageNotes = model.TriageNotes;

   
                entity.ChiefComplaint = model.ChiefComplaint;
                entity.HistoryOfPresentIllness = model.HistoryOfPresentIllness;

                entity.PastMedicalHistory = model.PastMedicalHistory;
                entity.PastSurgicalHistory = model.PastSurgicalHistory;
                entity.FamilyHistory = model.FamilyHistory;
                entity.PersonalHistory = model.PersonalHistory;
                entity.DrugHistory = model.DrugHistory;
                entity.AllergyHistory = model.AllergyHistory;

                entity.BloodPressure = model.BloodPressure;
                entity.PulseRate = SafeInt(model.PulseRate, 0, 250, "PulseRate");
                entity.RespiratoryRate = SafeInt(model.RespiratoryRate, 0, 80, "RespiratoryRate");
                entity.SpO2 = SafeInt(model.SpO2, 0, 100, "SpO2");

                entity.Temperature = SafeDecimal(model.Temperature, 20, 60, "Temperature");
                entity.Weight = SafeDecimal(model.Weight, 0, 300, "Weight");
                entity.Height = SafeDecimal(model.Height, 0, 250, "Height");
                entity.BMI = SafeDecimal(model.BMI, 5, 80, "BMI");
                entity.GRBS = SafeDecimal(model.GRBS, 20, 1000, "GRBS");

                entity.PainScore = SafeInt(model.PainScore, 0, 10, "PainScore");


                entity.GCSScore = SafeInt(model.GCSScore, 3, 15, "GCSScore");
                entity.Pupils = model.Pupils;


                entity.GeneralExamination = model.GeneralExamination;
                entity.CVSExamination = model.CVSExamination;
                entity.RSExamination = model.RSExamination;
                entity.AbdomenExamination = model.AbdomenExamination;
                entity.CNSExamination = model.CNSExamination;
                entity.LocalExamination = model.LocalExamination;

                entity.ProvisionalDiagnosis = model.ProvisionalDiagnosis;
                entity.InvestigationsOrdered = model.InvestigationsOrdered;
                entity.InvestigationResults = model.InvestigationResults;
                entity.ProceduresPerformed = model.ProceduresPerformed;
                entity.EmergencyTreatment = model.EmergencyTreatment;
                entity.MedicationsAdministered = model.MedicationsAdministered;
                entity.IVFluids = model.IVFluids;


                entity.FinalDiagnosis = model.FinalDiagnosis;

                entity.Disposition = model.Disposition;
                entity.AdmittedWard = model.AdmittedWard;
                entity.ReferredHospital = model.ReferredHospital;
                entity.DischargeAdvice = model.DischargeAdvice;
                entity.FollowUpAdvice = model.FollowUpAdvice;

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
                    model.PatientTreatment.CaseSheetId = entity.EMRId;

                    model.PatientTreatment.PatientId = model.PatientId;

                    model.PatientTreatment.DoctorId = model.DoctorId;


                    await _patientTreatmentService.SavePatientTreatments(
                        model.PatientTreatment
                    );


                    Console.WriteLine("STEP 10.1: Treatments saved");
                }


                await UpdateAllotmentAndReferralAsync(model, entity.EMRId);
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

        private async Task UpdateAllotmentAndReferralAsync(EMRCasesheetScreenVm model, int EMRId)
        {
            if (model.AllotId.HasValue)
            {
                var allotment = await _db.StudentAllotments
                    .FirstOrDefaultAsync(x => x.AllotId == model.AllotId.Value);

                if (allotment != null)
                {
                    allotment.CaserecordId = EMRId;
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

        public async Task<List<EMRCompletedCaseVm>> GetCompletedCases(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var fromParam = new SqlParameter("@FromDate", fromDate.Date);
                var toParam = new SqlParameter("@ToDate", toDate.Date);

                return await _db.Set<EMRCompletedCaseVm>()
                    .FromSqlRaw(
                        "EXEC usp_GetEMRCompletedCasesheets @FromDate, @ToDate",
                        fromParam,
                        toParam)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
        public async Task<EMRCasesheetScreenVm> GetCaseSheetById(int EMRId)
        {
            var param = new SqlParameter("@EMRId", EMRId);

            var data = await _db.Set<EMRCasesheetDbVm>()
                .FromSqlRaw("EXEC [usp_GetEMRCaseSheetById] @EMRId", param)
                .AsNoTracking()
                .ToListAsync();

            var model = data.FirstOrDefault();

            if (model == null)
                return new EMRCasesheetScreenVm();
            var DeptID = (int)Department.EMR;


            var departmentServices =
                await _patientTreatmentService.GetDepartmentServices(DeptID);


            var treatments =
                await _patientTreatmentService.GetPatientTreatments(model.EMRId);



            var patientTreatmentVM = new PatientTreatmentVM
            {

                CaseSheetId = model.EMRId,

                PatientId = model.PatientId,

                DeptId = DeptID,

                DoctorId = model.DoctorId,


                Services = departmentServices
                .Select(x => new PatientServiceVM
                {

                    ServiceID = x.ServiceID,

                    ServiceName = x.ServiceName,

                    Rate = x.Cost,

                    Quantity = 1,

                    DiscountPer = 0,

                    Selected = false

                })
                .ToList(),


                ExistingTreatments = treatments
                .Select(x => new PatientTreatmentVMItem
                {
                    TreatmentId = x.PatientTreatmentId, 
                    ServiceName = x.Service?.ServiceName ?? string.Empty,
                    ServiceID = x.ServiceID,

                    Rate = x.Rate,

                    Quantity = x.Quantity,

                    DiscountPer = x.DiscountPer,

                    Amount = x.Amount

                })
                .ToList(),

            };



            var medications = await _medicationService.GetActiveMedications();
            var existingMeds = await _medicationService.GetPatientMedications(model.PatientId);
            var followUp = await _followUpService.GetLatestFollowUpAsync(model.PatientId);
            var currentDeptId = await _followUpService.GetCurrentDepartmentIdAsync(model.PatientId);

            var departments = await _lookupService.GetDepartmentsAsync();
            var referrals = await _referralService.GetReferralsByPatientAsync(model.PatientId);
            var doctors = await _lookupService.GetDoctorsAsync();
            var students = await _lookupService.GetStudentsAsync();

            return new EMRCasesheetScreenVm
            {
                EMRId = model.EMRId,

                PatientId = model.PatientId,
                OpNo = model.OpNo ?? string.Empty,
                PatientName = model.PatientName ?? string.Empty,
                Age = model.Age ?? 0,
                Gender = model.Gender ?? string.Empty,
                //Phone = model.Phone,

                DoctorId = model.DoctorId,
                StudentId = model.StudentId,
                AllotId = model.AllotId,
                ReferredId = model.ReferredId,

                DoctorName = model.DoctorName ?? string.Empty,
                StudentName = model.StudentName ?? string.Empty,

                CaseDate = model.CaseDate,

                ArrivalMode = model.ArrivalMode,
                ArrivalTime = model.ArrivalTime,
                BroughtBy = model.BroughtBy,

                TriageCategory = model.TriageCategory,
                TriageTime = model.TriageTime,
                TriageNotes = model.TriageNotes,

                ChiefComplaint = model.ChiefComplaint,
                HistoryOfPresentIllness = model.HistoryOfPresentIllness,

                PastMedicalHistory = model.PastMedicalHistory,
                PastSurgicalHistory = model.PastSurgicalHistory,
                FamilyHistory = model.FamilyHistory,
                PersonalHistory = model.PersonalHistory,
                DrugHistory = model.DrugHistory,
                AllergyHistory = model.AllergyHistory,

                BloodPressure = model.BloodPressure,
                PulseRate = model.PulseRate,
                Temperature = model.Temperature,
                RespiratoryRate = model.RespiratoryRate,
                SpO2 = model.SpO2,
                Weight = model.Weight,
                Height = model.Height,
                BMI = model.BMI,
                GRBS = model.GRBS,
                PainScore = model.PainScore,
                GCSScore = model.GCSScore,
                Pupils = model.Pupils,

                GeneralExamination = model.GeneralExamination,
                CVSExamination = model.CVSExamination,
                RSExamination = model.RSExamination,
                AbdomenExamination = model.AbdomenExamination,
                CNSExamination = model.CNSExamination,
                LocalExamination = model.LocalExamination,

                ProvisionalDiagnosis = model.ProvisionalDiagnosis,
                InvestigationsOrdered = model.InvestigationsOrdered,
                InvestigationResults = model.InvestigationResults,
                ProceduresPerformed = model.ProceduresPerformed,
                EmergencyTreatment = model.EmergencyTreatment,
                MedicationsAdministered = model.MedicationsAdministered,
                IVFluids = model.IVFluids,

                FinalDiagnosis = model.FinalDiagnosis,
                Disposition = model.Disposition,
                AdmittedWard = model.AdmittedWard,
                ReferredHospital = model.ReferredHospital,
                DischargeAdvice = model.DischargeAdvice,
                FollowUpAdvice = model.FollowUpAdvice,

                IsSentForApproval1 = model.IsSentForApproval1,
                Approval1Status = model.Approval1Status,
                IsSentForApproval2 = model.IsSentForApproval2,
                Approval2Status = model.Approval2Status,

                MedicationMaster = medications,
                Medications = existingMeds.Any()
                    ? existingMeds
                    : new List<PatientMedicationVm> { new() },

                NextVisitDate = followUp?.FollowupDate,
                NextVisitTime = followUp?.FollowupTime,
                NextVisitDepartmentId = (followUp?.DeptId ?? 0) > 0
                    ? followUp!.DeptId
                    : currentDeptId,
                NextVisitDoctorId = followUp?.DoctorId,
                NextVisitStudentId = followUp?.StudentId,
                NextVisitReason = followUp?.FollowupReason,
                FollowUpNotes = followUp?.FollowupReason,
                Status = followUp?.Status ?? "Yet to visit",

                Doctors = doctors,
                Students = students,
                Departments = departments,


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
                    FromDeptId = currentDeptId ?? 0,
                    Departments = departments,
                    ExistingReferrals = referrals
                },

                PatientTreatment = patientTreatmentVM,

                ExistingTreatments = treatments,
            };
        }

        public async Task<List<EMRApprovalQueueVm>> GetApprovalQueue(DateTime fromDate, DateTime toDate)
        {
            var parameters = new[]
            {
                new SqlParameter("@FromDate", fromDate),
                new SqlParameter("@ToDate", toDate)
            };

            return await _db.Set<EMRApprovalQueueVm>().FromSqlRaw("EXEC [usp_GetEMRApprovalQueue] @FromDate,@ToDate", parameters).AsNoTracking().ToListAsync();
        }

        public async Task<string> ProcessApprovalFlow(int EMRId)
        {
            var emr = await _db.EMRCasesheets
                .FirstOrDefaultAsync(x => x.EMRId == EMRId);

            if (emr == null) return "Case sheet not found.";

            if (emr.IsSentForApproval1 != true)
            {
                emr.IsSentForApproval1 = true;
                emr.ModifiedDate = DateTime.Now;

                await _db.SaveChangesAsync();
                return "Case sheet sent for Approval 1 successfully.";
            }

            if (emr.IsSentForApproval1 == true && emr.Approval1Status != true && emr.IsSentForApproval2 != true)
            {
                emr.Approval1Status = true;
                emr.ModifiedDate = DateTime.Now;

                await _db.SaveChangesAsync();
                return "Approval 1 completed successfully.";
            }

            if (emr.IsSentForApproval1 == true && emr.Approval1Status == true && emr.IsSentForApproval2 != true)
            {
                emr.IsSentForApproval2 = true;
                emr.ModifiedDate = DateTime.Now;

                await _db.SaveChangesAsync();
                return "Case sheet sent for Approval 2 successfully.";
            }

            if (emr.IsSentForApproval1 == true && emr.Approval1Status == true && emr.IsSentForApproval2 == true && emr.Approval2Status != true)
            {
                emr.Approval2Status = true;
                emr.ModifiedDate = DateTime.Now;

                await _db.SaveChangesAsync();
                return "Approval 2 completed successfully.";
            }

            return "No action performed.";
        }

    }
}
