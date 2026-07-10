using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Entities.BillingDetails
{
    [Table("BillingAuditLog")]
    public class BillingAuditLog
    {
        [Key]
        public int AuditId { get; set; }


        public int BillId { get; set; }


        public string ActionType { get; set; } = string.Empty;


        public string? OldValue { get; set; }


        public string? NewValue { get; set; }


        public int? ActionBy { get; set; }


        public DateTime ActionDate { get; set; } = DateTime.Now;


        // Navigation
        public Billing Billing { get; set; } = null!;
    }
}
