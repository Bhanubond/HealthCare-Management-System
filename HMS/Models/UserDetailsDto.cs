using System.ComponentModel.DataAnnotations;

namespace HMS.Models
{
    public class UserDetailsDto
    {
        [ScaffoldColumn(false)]
        public int UserId { get; set; }

        [Display(Name = "User Name")]
        public string UserName { get; set; } = string.Empty;

        [Display(Name = "Name")]
        public string? Name { get; set; }

        [Display(Name = "Department")]
        public string? DeptName { get; set; }

        [Display(Name = "Category")]
        public int? CategoryId { get; set; }

        [Display(Name = "Active")]
        public bool Active { get; set; }

        [Display(Name = "Enable View All Patient")]
        public bool EnableViewAllPatient { get; set; }

        [Display(Name = "Password Reset")]
        public bool PasswordReset { get; set; }

        [Display(Name = "Enable Live Chat")]
        public bool EnableLiveChat { get; set; }

        [Display(Name = "Is Online")]
        public bool IsOnline { get; set; }

        [Display(Name = "Created By")]
        public string? CreatedBy { get; set; }

        [Display(Name = "Created Date")]
        public DateTime? CreatedDate { get; set; }

        [Display(Name = "Modified By")]
        public string? ModifiedBy { get; set; }

        [Display(Name = "Modified Date")]
        public DateTime? ModifiedDate { get; set; }

        [Display(Name = "Last Seen At")]
        public DateTime? LastSeenAt { get; set; }
    }
}
