using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Entities
{
    [Table("GMCasesheet")]
    public class GMCasesheet : EntityBase
    {
        [Key]
        public int GMID { get; set; }

        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public int StudentId { get; set; }
        public int? AllotId { get; set; }
        public int? ReferredId { get; set; }

        public string? OpNo { get; set; }
        public DateTime CaseDate { get; set; }

        public string? ChiefComplaint { get; set; }
        public string? Symptoms { get; set; }
        public string? Diagnosis { get; set; }
        public string? Prescription { get; set; }
        public string? Notes { get; set; }

        public bool? IsSentForApproval1 { get; set; }
        public bool? Approval1Status { get; set; }

        public bool? IsSentForApproval2 { get; set; }
        public bool? Approval2Status { get; set; }

        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public string? CreatedSystem { get; set; }

        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public string? ModifiedSystem { get; set; }
    }
}
