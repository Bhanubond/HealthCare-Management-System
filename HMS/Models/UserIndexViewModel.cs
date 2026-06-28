using System.ComponentModel.DataAnnotations;

namespace HMS.Models
{
    public class UserIndexViewModel
    {
        [ScaffoldColumn(false)]
        public int UserId { get; set; }

        [Display(Name = "User Name")]
        public string UserName { get; set; } = string.Empty;

        [Display(Name = "Name")]
        public string? Name { get; set; }

        [Display(Name = "Department")]
        public string? DeptName { get; set; }

        [Display(Name = "Active")]
        public bool Active { get; set; }
    }
}
