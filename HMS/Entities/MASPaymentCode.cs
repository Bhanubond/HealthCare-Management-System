using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Entities
{
    [Table("MASPaymentCode")]
    public class MASPaymentCode : EntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PaymodeId { get; set; }

        [Required]
        [StringLength(50)]
        public string PaymodeCode { get; set; }

        [Required]
        [StringLength(100)]
        public string PaymodeName { get; set; }

        public bool IsActive { get; set; } = true;

        public bool DelInd { get; set; } = false;

        public int? DisplayOrder { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public int? CreatedBy { get; set; }
    }
}
