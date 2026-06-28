using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Entities
{
    [Table("MASCategory")]
    public class MASCategory : EntityBase
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(200)]
        public string CategoryName { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? DiscountPer { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? InvestigationDiscountPer { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? TreatmentDiscountPer { get; set; }

        public bool IsActive { get; set; } = true;

        public bool DelInd { get; set; } = false;

        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? CreatedBy { get; set; }
    }
}