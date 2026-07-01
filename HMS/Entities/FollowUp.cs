using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Entities
{
    [Table("FollowUp")]
    public class FollowUp
    {
        [Key]
        public int FollowupId { get; set; }

        public int PatientId { get; set; }

        public DateTime FollowupDate { get; set; }

        public TimeSpan? FollowupTime { get; set; }

        public int DeptId { get; set; }

        public string? FollowupReason { get; set; }

        public int? DoctorId { get; set; }

        public int? StudentId { get; set; }

        public int? RevisitId { get; set; }

        public string? Status { get; set; }

        public string? IgnoreReason { get; set; }

        public int ReferredTreatmentId { get; set; }

        public string CreatedBy { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }

        public string CreatedSystem { get; set; } = string.Empty;

        public string? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string? ModifiedSystem { get; set; }

        public bool IsCancelled { get; set; }

        public string? CancelledReason { get; set; }

        public DateTime? CancelledDate { get; set; }

        public string? CancelledBy { get; set; }

        public string? CancelledSystem { get; set; }

        // Navigation Properties (Optional)
        // public virtual Patient Patient { get; set; }
        // public virtual Department Department { get; set; }
        // public virtual Revisit? Revisit { get; set; }
        // public virtual Treatment? ReferredTreatment { get; set; }
    }
}
