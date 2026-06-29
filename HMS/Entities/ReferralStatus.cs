using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Entities
{
    [Table("ReferralStatus")]
    public class ReferralStatus : EntityBase
    {
        [Key]
        public int ReferredId { get; set; }

        public int PatientId { get; set; }

        public int FromDeptId { get; set; }

        public int? ToDeptId { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? TreatmentDate { get; set; }

        public string? ReferredReason { get; set; }
        public string? AllotmentStatus { get; set; }

        public string? TreatmentStatus { get; set; }

        public string? VisitType { get; set; }

        public int? RevisitId { get; set; }

        public int? CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string? CreatedSystem { get; set; }

        public string? ModifiedSystem { get; set; }
    }
}
