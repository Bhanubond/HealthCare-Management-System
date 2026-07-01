using Microsoft.AspNetCore.Mvc.Rendering;

namespace HMS.Models
{
    public class FollowUpVm
    {
        public int FollowupId { get; set; }

        public int PatientId { get; set; }

        public DateTime FollowupDate { get; set; }

        public TimeSpan? FollowupTime { get; set; }

        public int DeptId { get; set; }

        public string? FollowupReason { get; set; }

        public int? DoctorId { get; set; }

        public int? StudentId { get; set; }

        public string Status { get; set; } = "Yet to visit";

        public int? RevisitId { get; set; }

        public string? IgnoreReason { get; set; }

        public int ReferredTreatmentId { get; set; }

        public List<SelectListItem> Doctors { get; set; } = new();

        public List<SelectListItem> Students { get; set; } = new();
    }
}
