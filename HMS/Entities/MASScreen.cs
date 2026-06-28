using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Entities
{
    [Table("MASScreen")]
    public class MASScreen
    {
        [Key]
        public int ScreenId { get; set; }
        public string ScreenName { get; set; }
        public string ScreenDisplayName { get; set; }
        public string Title { get; set; }

        public string ControllerName { get; set; }
        public string ActionName { get; set; }

        public bool DelInd { get; set; }
        public int? OrderDisplay { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
    }
}
