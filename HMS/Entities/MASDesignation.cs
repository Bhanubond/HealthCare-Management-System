using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Entities
{
    [Table("MASDesignation")]
    public class MASDesignation
    {
        [Key]
        public int DesignationId { get; set; }
        [Display (Name = "Designation Name")]
        public string DesignationName { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
    }
}
