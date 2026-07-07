using Microsoft.AspNetCore.Mvc.Rendering;

namespace HMS.Models
{
    public class FollowUpSaveVm
    {
        public int PatientId { get; set; }

        public DateTime? FollowupDate { get; set; }

        public TimeSpan? FollowupTime { get; set; }

        public int DeptId { get; set; }

        public string? FollowupReason { get; set; }

        public int? DoctorId { get; set; }

        public int? StudentId { get; set; }

        public string Status { get; set; } = "Yet to visit";

        public int ReferredTreatmentId { get; set; }

        public List<SelectListItem> Doctors { get; set; } = new();
        public List<SelectListItem> Departments { get; set; } = new();

        public List<SelectListItem> Students { get; set; } = new();

        public DateTime? NextVisitDate { get; set; }
        public TimeSpan? NextVisitTime { get; set; }
        public int? NextVisitDepartmentId { get; set; }
        public int? NextVisitDoctorId { get; set; }
        public int? NextVisitStudentId { get; set; }
        public string? NextVisitReason { get; set; }
        public string? FollowUpNotes { get; set; }
    }
}
