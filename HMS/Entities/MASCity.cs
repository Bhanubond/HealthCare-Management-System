using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Entities
{
    [Table("MASCities")]
    public class MASCity
    {
        [Key]
        public int CityId { get; set; }
        public int MandalId { get; set; }
        public string CityName { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsDefault { get; set; }

        public MASMandal? Mandal { get; set; }
    }
}
