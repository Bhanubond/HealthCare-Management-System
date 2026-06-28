using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Entities
{
    [Table("MASMandal")]
    public class MASMandal
    {
        [Key]
        public int MandalId { get; set; }
        public int StateId { get; set; }
        public string MandalName { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsDefault { get; set; }

        public MASState? State { get; set; }
        public ICollection<MASCity>? Cities { get; set; }
    }
}
