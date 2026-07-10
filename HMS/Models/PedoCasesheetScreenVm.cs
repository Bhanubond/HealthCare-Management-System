using HMS.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Models
{
    public class PedoCasesheetScreenVm
    {
        public int PedoID { get; set; }
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

        public string? ChiefComplaint { get; set; }

        public string? HistoryOfPresentIllness { get; set; }

        public string? PastMedicalHistory { get; set; }

        public string? FamilyHistory { get; set; }

        public string? BirthHistory { get; set; }

        public decimal? BirthWeight { get; set; }

        public string? ImmunizationStatus { get; set; }

        public decimal? Weight { get; set; }

        public decimal? Height { get; set; }

        public string? GrowthStatus { get; set; }

        public decimal? Temperature { get; set; }

        public int? PulseRate { get; set; }

        public int? RespiratoryRate { get; set; }

        public int? SpO2 { get; set; }

        public string? GeneralExamination { get; set; }

        public string? SystemicExamination { get; set; }

        public string? Diagnosis { get; set; }

        public string? Prescription { get; set; }

        public string? Advice { get; set; }

        public string? Notes { get; set; }

        public bool IsSentForApproval1 { get; set; } = false;

        public bool Approval1Status { get; set; } = false;

        public bool IsSentForApproval2 { get; set; } = false;

        public bool Approval2Status { get; set; } = false;

        public DateTime CreatedDate { get; set; }

        public int? CreatedBy { get; set; }

        public string? CreatedSystem { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public int? ModifiedBy { get; set; }

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
    }
}
