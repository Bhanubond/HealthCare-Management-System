using System.ComponentModel.DataAnnotations;

namespace HMS.Models
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(50)]
        [Display(Name = "Username")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and Confirm Password do not match.")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Display(Name = "Student Id")]
        public long? StudentId { get; set; }

        [Display(Name = "Doctor Id")]
        public long? DoctorId { get; set; }

        public int? CategoryId { get; set; }
    }
}
