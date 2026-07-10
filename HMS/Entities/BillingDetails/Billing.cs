using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Entities.BillingDetails
{
    [Table("Billing")]
    public class Billing
    {
        [Key]
        public int BillId { get; set; }

        public string BillNo { get; set; } = string.Empty;

        public int CaseSheetId { get; set; }

        public int PatientId { get; set; }


        public decimal GrossAmount { get; set; } = 0;

        public decimal DiscountAmount { get; set; } = 0;

        public decimal NetAmount { get; set; } = 0;


        public decimal PaidAmount { get; set; } = 0;

        public decimal BalanceAmount { get; set; } = 0;


        public decimal RefundAmount { get; set; } = 0;


        public bool IsPaid { get; set; } = false;

        public bool IsCancelled { get; set; } = false;


        public string? CancelReason { get; set; }

        public int? CancelledBy { get; set; }

        public DateTime? CancelledDate { get; set; }


        public DateTime BillDate { get; set; } = DateTime.Now;


        public int? CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;


        public bool IsDelInd { get; set; } = false;


        // Navigation
        public ICollection<BillingDetails> BillingDetails { get; set; } = new List<BillingDetails>();

        public ICollection<PaymentDetail> Payments { get; set; } = new List<PaymentDetail>();

        public ICollection<BillingAuditLog> AuditLogs { get; set; } = new List<BillingAuditLog>();
    }
}
