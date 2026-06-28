using System.ComponentModel.DataAnnotations;
using HMS.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HMS.Models
{
    public class UserFormViewModel
    {
        public int UserId { get; set; }

        [Display(Name = "User Name")]
        [Required]
        public string UserName { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Student")]
        public long? StudentId { get; set; }

        [Display(Name = "Doctor")]
        public long? DoctorId { get; set; }

        [Display(Name = "Category")]
        public int? CategoryId { get; set; }

        public bool Active { get; set; }

        [Display(Name = "Enable View All Patient")]
        public bool EnableViewAllPatient { get; set; }

        [Display(Name = "Password Reset")]
        public bool PasswordReset { get; set; }

        [Display(Name = "Enable Live Chat")]
        public bool EnableLiveChat { get; set; }

        public bool IsOnline { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public DateTime? LastSeenAt { get; set; }

        public IEnumerable<SelectListItem> StudentOptions { get; set; } = Array.Empty<SelectListItem>();

        public IEnumerable<SelectListItem> DoctorOptions { get; set; } = Array.Empty<SelectListItem>();

        public Users ToEntity()
        {
            return new Users
            {
                UserId = UserId,
                UserName = UserName,
                Password = Password,
                StudentId = StudentId,
                DoctorId = DoctorId,
                CategoryId = CategoryId,
                Active = Active,
                EnableViewAllPatient = EnableViewAllPatient,
                PasswordReset = PasswordReset,
                EnableLiveChat = EnableLiveChat,
                IsOnline = IsOnline,
                CreatedBy = CreatedBy,
                CreatedDate = CreatedDate,
                ModifiedBy = ModifiedBy,
                ModifiedDate = ModifiedDate,
                LastSeenAt = LastSeenAt
            };
        }
    }
}
