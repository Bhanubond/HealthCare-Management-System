using HMS.Entities;
using HMS.Entities.BillingDetails;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Models
{
    public class EMRCasesheetScreenVm
    {
        public int EMRId { get; set; }

        public int PatientId { get; set; }
        public string? PatientName { get; set; }
        public int Age { get; set; }
        public string? Gender { get; set; }

        public int DoctorId { get; set; }

        public int StudentId { get; set; }
        public int Phone { get; set; }

        public string? DoctorName { get; set; }
        public string? StudentName { get; set; }

        public int? AllotId { get; set; }

        public int? ReferredId { get; set; }

        [StringLength(50)]
        public string? OpNo { get; set; }

        public DateTime CaseDate { get; set; }

        [StringLength(50)]
        public string? ArrivalMode { get; set; }

        public DateTime? ArrivalTime { get; set; }

        [StringLength(200)]
        public string? BroughtBy { get; set; }

        [StringLength(20)]
        public string? TriageCategory { get; set; }

        public DateTime? TriageTime { get; set; }

        public string? TriageNotes { get; set; }

        public string? ChiefComplaint { get; set; }

        public string? HistoryOfPresentIllness { get; set; }

        public string? PastMedicalHistory { get; set; }

        public string? PastSurgicalHistory { get; set; }

        public string? FamilyHistory { get; set; }

        public string? PersonalHistory { get; set; }

        public string? DrugHistory { get; set; }

        public string? AllergyHistory { get; set; }

        [StringLength(20)]
        public string? BloodPressure { get; set; }

        public int? PulseRate { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? Temperature { get; set; }

        public int? RespiratoryRate { get; set; }

        public int? SpO2 { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? Weight { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? Height { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? BMI { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? GRBS { get; set; }

        public int? PainScore { get; set; }

        public int? GCSScore { get; set; }

        [StringLength(100)]
        public string? Pupils { get; set; }

        public string? GeneralExamination { get; set; }

        public string? CVSExamination { get; set; }

        public string? RSExamination { get; set; }

        public string? AbdomenExamination { get; set; }

        public string? CNSExamination { get; set; }

        public string? LocalExamination { get; set; }

        public string? ProvisionalDiagnosis { get; set; }

        public string? InvestigationsOrdered { get; set; }

        public string? InvestigationResults { get; set; }

        public string? ProceduresPerformed { get; set; }

        public string? EmergencyTreatment { get; set; }

        public string? MedicationsAdministered { get; set; }

        public string? IVFluids { get; set; }

        public string? FinalDiagnosis { get; set; }

        [StringLength(50)]
        public string? Disposition { get; set; }

        [StringLength(100)]
        public string? AdmittedWard { get; set; }

        [StringLength(200)]
        public string? ReferredHospital { get; set; }

        public string? DischargeAdvice { get; set; }

        public string? FollowUpAdvice { get; set; }

        public bool IsSentForApproval1 { get; set; }

        public bool Approval1Status { get; set; }

        public bool IsSentForApproval2 { get; set; }

        public bool Approval2Status { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? CreatedBy { get; set; }

        [StringLength(100)]
        public string? CreatedSystem { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public int? ModifiedBy { get; set; }

        [StringLength(100)]
        public string? ModifiedSystem { get; set; }


        public DateTime? NextVisitDate { get; set; }
        public TimeSpan? NextVisitTime { get; set; }
        public int? NextVisitDepartmentId { get; set; }
        public int? NextVisitDoctorId { get; set; }
        public int? NextVisitStudentId { get; set; }
        public string? NextVisitReason { get; set; }
        public string? FollowUpNotes { get; set; }
        public string? Status { get; set; }

        [NotMapped]
        public List<PatientMedicationVm> Medications { get; set; } = new();

        [NotMapped]
        public List<MASMedication> MedicationMaster { get; set; } = new();

        [NotMapped]
        public List<SelectListItem> Doctors { get; set; } = new();

        [NotMapped]
        public List<SelectListItem> Students { get; set; } = new();

        [NotMapped]
        public List<SelectListItem> Departments { get; set; } = new();

        [NotMapped]
        public List<int> SelectedToDeptIds { get; set; } = new();

        [NotMapped]
        public ReferralStatusVm ReferralStatus { get; set; } = new();

        [NotMapped]
        public Dictionary<int, string> Reasons { get; set; } = new();

        public FollowUpSaveVm FollowUpSaveVm { get; set; } = new();

        public PatientTreatmentVM? PatientTreatment { get; set; }

        public List<PatientTreatment> ExistingTreatments { get; set; }
            = new();
    }
}
