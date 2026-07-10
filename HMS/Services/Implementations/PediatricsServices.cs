using HMS.Data;
using HMS.Entities;
using HMS.Models;
using HMS.Services.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

using HMS.Common;

namespace HMS.Services.Implementations
{
    public class PediatricsServices : IPediatricsServices
    {
        private readonly HmsDbContext _db;
        private readonly ILookupService _lookupService;
        private readonly IMedicationService _medicationService;
        private readonly IFollowUpService _followUpService;
        private readonly IReferralStatusService _referralService;
        private readonly IPatientTreatmentService _patientTreatmentService;

        public PediatricsServices(HmsDbContext db, ILookupService lookupService, IMedicationService medicationService, IReferralStatusService referralService, IFollowUpService followUpService, IPatientTreatmentService patientTreatmentService)
        {
            _db = db;
            _lookupService = lookupService;
            _medicationService = medicationService;
            _referralService = referralService;
            _followUpService = followUpService;
            _patientTreatmentService = patientTreatmentService;
        }
        public async Task<PedoCasesheetScreenVm> GetTreatmentScreenAsync(int DeptId, int patientId)
        {
            var patient = await _lookupService.GetCaseSheetPatient(patientId);
            var medications = await _medicationService.GetActiveMedications();
            var context = await _lookupService.GetLatestTreatmentContextAsync(DeptId, patientId);
            var currentDept = await _followUpService.GetCurrentDepartmentIdAsync(patientId);
            var departmentServices = await _patientTreatmentService.GetDepartmentServices(DeptId);
            return new PedoCasesheetScreenVm
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


        public async Task SaveCaseSheet(PedoCasesheetScreenVm model)
        {
            await SaveOrUpdateCaseSheetAsync(model, isUpdate: false);
        }
        public async Task UpdateCaseSheet(PedoCasesheetScreenVm model)
        {
            await SaveOrUpdateCaseSheetAsync(model, isUpdate: true);
        }

        private async Task SaveOrUpdateCaseSheetAsync(PedoCasesheetScreenVm model, bool isUpdate)
        {
            await using var tx = await _db.Database.BeginTransactionAsync();
            var now = DateTime.Now;

            try
            {
                Console.WriteLine("STEP 1: Transaction started");

                PediatricsCasesheet? entity;

                if (isUpdate)
                {
                    entity = await _db.PediatricsCasesheets.FirstOrDefaultAsync(x => x.PedoID == model.PedoID);

                    if (entity == null)
                    {
                        Console.WriteLine("ERROR: Entity not found for update");
                        throw new Exception("CaseSheet not found");
                    }
                }
                else
                {
                    entity = new PediatricsCasesheet
                    {
                        CreatedDate = now,
                        CreatedBy = 1,
                        CreatedSystem = Environment.MachineName
                    };

                    _db.PediatricsCasesheets.Add(entity);
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

                entity.ChiefComplaint = model.ChiefComplaint;
                entity.HistoryOfPresentIllness = model.HistoryOfPresentIllness;
                entity.PastMedicalHistory = model.PastMedicalHistory;
                entity.FamilyHistory = model.FamilyHistory;
                entity.BirthHistory = model.BirthHistory;
                entity.BirthWeight = SafeDecimal(model.BirthWeight, 0, 10, "BirthWeight");
                entity.ImmunizationStatus = model.ImmunizationStatus;

                entity.Weight = SafeDecimal(model.Weight, 0, 300, "Weight");
                entity.Height = SafeDecimal(model.Height, 0, 250, "Height");
                entity.GrowthStatus = model.GrowthStatus;


                entity.PulseRate = SafeInt(model.PulseRate, 0, 250, "PulseRate");
                entity.RespiratoryRate = SafeInt(model.RespiratoryRate, 0, 80, "RespiratoryRate");
                entity.SpO2 = SafeInt(model.SpO2, 0, 100, "SpO2");
                entity.Temperature = SafeDecimal(model.Temperature, 20, 60, "Temperature");

                entity.GeneralExamination = model.GeneralExamination;
                entity.SystemicExamination = model.SystemicExamination;

                entity.Diagnosis = model.Diagnosis;
                entity.Prescription = model.Prescription;
                entity.Advice = model.Advice;
                entity.Notes = model.Notes;


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
                    model.PatientTreatment.CaseSheetId = entity.PedoID;
                    model.PatientTreatment.PatientId = model.PatientId;
                    model.PatientTreatment.DoctorId = model.DoctorId;

                    await _patientTreatmentService.SavePatientTreatments(model.PatientTreatment);
                    Console.WriteLine("STEP 11.1: Treatments saved");
                }

                await UpdateAllotmentAndReferralAsync(model, entity.PedoID);
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

        private async Task UpdateAllotmentAndReferralAsync(PedoCasesheetScreenVm model, int PedoID)
        {
            if (model.AllotId.HasValue)
            {
                var allotment = await _db.StudentAllotments
                    .FirstOrDefaultAsync(x => x.AllotId == model.AllotId.Value);

                if (allotment != null)
                {
                    allotment.CaserecordId = PedoID;
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

        public async Task<List<PedoCompletedCaseVm>> GetCompletedCases(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var fromParam = new SqlParameter("@FromDate", fromDate.Date);
                var toParam = new SqlParameter("@ToDate", toDate.Date);

                return await _db.Set<PedoCompletedCaseVm>()
                    .FromSqlRaw(
                        "EXEC [usp_GetPedoCompletedCasesheets] @FromDate, @ToDate",
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
        public async Task<PedoCasesheetScreenVm> GetCaseSheetById(int PedoID)
        {
            var param = new SqlParameter("@PedoID", PedoID);

            var data = await _db.Set<PedoCasesheetDbVm>()
                .FromSqlRaw("EXEC [usp_GetPedoCaseSheetById] @PedoID", param)
                .AsNoTracking()
                .ToListAsync();

            var model = data.FirstOrDefault();

            if (model == null)
                return new PedoCasesheetScreenVm();

            var medications = await _medicationService.GetActiveMedications();
            var existingMeds = await _medicationService.GetPatientMedications(model.PatientId);
            var followUp = await _followUpService.GetLatestFollowUpAsync(model.PatientId);
            var currentDeptId = await _followUpService.GetCurrentDepartmentIdAsync(model.PatientId);
            var departmentServices = await _patientTreatmentService.GetDepartmentServices((int)Department.PED);
            var treatments = await _patientTreatmentService.GetPatientTreatments(model.PedoID);

            var departments = await _lookupService.GetDepartmentsAsync();
            var referrals = await _referralService.GetReferralsByPatientAsync(model.PatientId);
            var doctors = await _lookupService.GetDoctorsAsync();
            var students = await _lookupService.GetStudentsAsync();

            return new PedoCasesheetScreenVm
            {
                PedoID = model.PedoID,

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

                CaseDate = model.CaseDate,

                ChiefComplaint = model.ChiefComplaint,
                HistoryOfPresentIllness = model.HistoryOfPresentIllness,
                PastMedicalHistory = model.PastMedicalHistory,
                FamilyHistory = model.FamilyHistory,
                BirthHistory = model.BirthHistory,
                BirthWeight = model.BirthWeight,
                ImmunizationStatus = model.ImmunizationStatus,

                Weight = model.Weight,
                Height = model.Height,
                GrowthStatus = model.GrowthStatus,

                Temperature = model.Temperature,
                PulseRate = model.PulseRate,
                RespiratoryRate = model.RespiratoryRate,
                SpO2 = model.SpO2,

                GeneralExamination = model.GeneralExamination,
                SystemicExamination = model.SystemicExamination,

                Diagnosis = model.Diagnosis,
                Prescription = model.Prescription,
                Advice = model.Advice,
                Notes = model.Notes,

                IsSentForApproval1 = model.IsSentForApproval1,
                Approval1Status = model.Approval1Status,
                IsSentForApproval2 = model.IsSentForApproval2,
                Approval2Status = model.Approval2Status,

                MedicationMaster = medications,
                Medications = existingMeds.Any() ? existingMeds : new List<PatientMedicationVm> { new() },

                NextVisitDate = followUp?.FollowupDate,
                NextVisitTime = followUp?.FollowupTime,
                NextVisitDepartmentId = (followUp?.DeptId ?? 0) > 0 ? followUp!.DeptId : currentDeptId,
                NextVisitDoctorId = followUp?.DoctorId,
                NextVisitStudentId = followUp?.StudentId,
                NextVisitReason = followUp?.FollowupReason,
                FollowUpNotes = followUp?.FollowupReason,
                Status = followUp?.Status ?? "Yet to visit",

                Doctors = doctors,
                Students = students,
                Departments = departments,
                PatientTreatment = new PatientTreatmentVM
                {
                    CaseSheetId = model.PedoID,
                    PatientId = model.PatientId,
                    DeptId = (int)Department.PED,
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

                ReferralStatus = new ReferralStatusVm
                {
                    PatientId = model.PatientId,
                    FromDeptId = currentDeptId ?? 0,
                    Departments = departments,
                    ExistingReferrals = referrals
                }
            };
        }

        public async Task<List<PedoApprovalQueueVm>> GetApprovalQueue(DateTime fromDate, DateTime toDate)
        {
            var parameters = new[]
            {
                new SqlParameter("@FromDate", fromDate),
                new SqlParameter("@ToDate", toDate)
            };

            return await _db.Set<PedoApprovalQueueVm>().FromSqlRaw("EXEC [usp_GetPedoApprovalQueue] @FromDate,@ToDate", parameters).AsNoTracking().ToListAsync();
        }

        public async Task<string> ProcessApprovalFlow(int PedoID)
        {
            var pedo = await _db.PediatricsCasesheets
                .FirstOrDefaultAsync(x => x.PedoID == PedoID);

            if (pedo == null) return "Case sheet not found.";

            if (pedo.IsSentForApproval1 != true)
            {
                pedo.IsSentForApproval1 = true;
                pedo.ModifiedDate = DateTime.Now;

                await _db.SaveChangesAsync();
                return "Case sheet sent for Approval 1 successfully.";
            }

            if (pedo.IsSentForApproval1 == true && pedo.Approval1Status != true && pedo.IsSentForApproval2 != true)
            {
                pedo.Approval1Status = true;
                pedo.ModifiedDate = DateTime.Now;

                await _db.SaveChangesAsync();
                return "Approval 1 completed successfully.";
            }

            if (pedo.IsSentForApproval1 == true && pedo.Approval1Status == true && pedo.IsSentForApproval2 != true)
            {
                pedo.IsSentForApproval2 = true;
                pedo.ModifiedDate = DateTime.Now;

                await _db.SaveChangesAsync();
                return "Case sheet sent for Approval 2 successfully.";
            }

            if (pedo.IsSentForApproval1 == true && pedo.Approval1Status == true && pedo.IsSentForApproval2 == true && pedo.Approval2Status != true)
            {
                pedo.Approval2Status = true;
                pedo.ModifiedDate = DateTime.Now;

                await _db.SaveChangesAsync();
                return "Approval 2 completed successfully.";
            }

            return "No action performed.";
        }

    }
}
