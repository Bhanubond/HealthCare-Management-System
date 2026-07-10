using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Entities
{
    [Table("TreatmentServices")]
    public class TreatmentServices
    {
        [Key]
        public int ServiceID { get; set; }

        [Required]
        [StringLength(20)]
        public string ServiceCode { get; set; }

        [Required]
        [StringLength(100)]
        public string ServiceName { get; set; }

        [Required]
        public int DeptId { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Cost { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Correct Foreign Key Mapping
        [ForeignKey("DeptId")]
        public MASDepartment? Department { get; set; }
    }
}
