using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Entities
{
    [Table("MASMedication")]
    public class MASMedication
    {
        [Key]
        public int MedId { get; set; }

        public int? DeptId { get; set; }  

        [Required]
        [StringLength(200)]
        public string Medication { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Price { get; set; }

        public bool IsActive { get; set; } = true;

        public bool DelInd { get; set; } = false;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public int? CreatedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public int? ModifiedBy { get; set; }

    }
}
