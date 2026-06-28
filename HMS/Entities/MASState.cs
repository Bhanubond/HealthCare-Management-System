using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Entities
{
    [Table("MASState")]
    public class MASState
    {
        [Key]
        public int StateId { get; set; }
        public int CountryId { get; set; }
        public string StateName { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsDefault { get; set; }

        public MASCountry? Country { get; set; }
        public ICollection<MASMandal>? Mandals { get; set; }
    }
}
