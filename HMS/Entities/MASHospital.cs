using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Entities
{
    [Table("MASHospital")]
    public class MASHospital : EntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int HospitalId { get; set; }

        [StringLength(200)]
        public string HospitalName { get; set; }

        [StringLength(200)]
        public string TagLine { get; set; }

        [StringLength(500)]
        public string Address { get; set; }

        [StringLength(100)]
        public string City { get; set; }

        [StringLength(100)]
        public string State { get; set; }

        [StringLength(100)]
        public string Country { get; set; }

        [StringLength(50)]
        public string Phone { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(100)]
        public string Website { get; set; }

        [StringLength(500)]
        public string LogoPath { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal OPFee { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
