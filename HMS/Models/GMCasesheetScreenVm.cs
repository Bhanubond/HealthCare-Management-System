using HMS.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HMS.Models
{
    public class GMCasesheetScreenVm
    {
        // ---------------- PATIENT INFO ----------------
        public int GMID { get; set; }
        public int PatientId { get; set; }
        public string? OpNo { get; set; }
        public string? PatientName { get; set; }
        public int Age { get; set; }
        public string? Gender { get; set; }

        public string? DoctorName { get; set; }
        public string? StudentName { get; set; }

        public int DoctorId { get; set; }
        public int StudentId { get; set; }
        public int? AllotId { get; set; }
        public int? ReferredId { get; set; }

        // ---------------- CASE SHEET ----------------
        public string? ChiefComplaint { get; set; }
        public string? Symptoms { get; set; }
        public string? Diagnosis { get; set; }
        public string? Prescription { get; set; }
        public string? Notes { get; set; }

        public List<PatientMedicationVm> Medications { get; set; } = new();

        // dropdown list
        public List<MASMedication> MedicationMaster { get; set; } = new();

        public DateTime? NextVisitDate { get; set; }
        public TimeSpan? NextVisitTime { get; set; }
        public int? NextVisitDepartmentId { get; set; }
        public int? NextVisitDoctorId { get; set; }
        public int? NextVisitStudentId { get; set; }
        public string? NextVisitReason { get; set; }
        public string? FollowUpNotes { get; set; }
        public string? Status { get; set; }

        public bool IsSentForApproval1 { get; set; } = false;
        public bool Approval1Status { get; set; } = false;
        public bool IsSentForApproval2 { get; set; } = false;
        public bool Approval2Status { get; set; } = false;

        public List<SelectListItem> Doctors { get; set; } = new();
        public List<SelectListItem> Students { get; set; } = new();
        public List<SelectListItem> Departments { get; set; } = new();

        public ReferralStatusVm ReferralStatus { get; set; } = new();
    }
}
