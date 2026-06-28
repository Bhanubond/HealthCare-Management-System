using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Entities
{
    [Table("MASCountry")]
    public class MASCountry
    {
        [Key]
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsDefault { get; set; }

        public ICollection<MASState>? States { get; set; }
    }
}
