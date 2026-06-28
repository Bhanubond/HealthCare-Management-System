using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Entities
{
    [Table("UserLoginLog")]
    public class UserLoginLog
    {
        [Key]
        public long LoginLogId { get; set; }

        public int? UserId { get; set; }

        public string UserName { get; set; } = string.Empty;

        public DateTime LoginTime { get; set; }

        public DateTime? LogoutTime { get; set; }

        public string LoginStatus { get; set; } = string.Empty;

        public string? IPAddress { get; set; }

        public string? MachineName { get; set; }

        public string? BrowserName { get; set; }

        public string? SessionId { get; set; }

        public string? Remarks { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
