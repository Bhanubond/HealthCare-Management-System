using System.ComponentModel.DataAnnotations;

namespace HMS.Models
{
    public class ScreenVM
    {
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
        public bool CanCreate { get;  set; }
        public bool CanRead { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
    }
}
