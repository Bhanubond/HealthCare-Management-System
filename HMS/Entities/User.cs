using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Entities
{
    [Table("Users")]
    public class Users : EntityBase
    {
        [Key]
        public int UserId { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public long? StudentId { get; set; }

        public long? DoctorId { get; set; }

        public int? CategoryId { get; set; }

        public bool Active { get; set; }

        public bool EnableViewAllPatient { get; set; }

        public bool PasswordReset { get; set; }

        public bool EnableLiveChat { get; set; }

        public bool IsOnline { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public DateTime? LastSeenAt { get; set; }
    }

   
        //public class UserIndexDto
        //{
        //    public int UserId { get; set; }

        //    public string UserName { get; set; } = string.Empty;

        //    public string? Name { get; set; }     // StudentName / DoctorName

        //    public string? DeptName { get; set; }

        //    public bool Active { get; set; }
        //}
    
}
