using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Entities
{
    [Table("StudentAllotment")]
    public class StudentAllotment : EntityBase
    {
        [Key]
        public int AllotId { get; set; }

        public DateTime? AllotDate { get; set; }

        public int? PatientId { get; set; }
        public int? ReferredId { get; set; }
        public int? DeptId { get; set; }
        public int? StudentId { get; set; }
        public int? DoctorId { get; set; }
        public int? CaserecordId { get; set; }

        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        // 🔴 FIXED (nullable strings)
        public string? CreatedSystem { get; set; }
        public string? ModifiedSystem { get; set; }
    }
}
