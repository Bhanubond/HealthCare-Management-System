using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Entities.BillingDetails
{
    [Table("PaymentDetail")]
    public class PaymentDetail
    {
        [Key]
        public int PaymentId { get; set; }

        public int BillId { get; set; }


        public string PaymentMode { get; set; } = string.Empty;


        public decimal Amount { get; set; } = 0;


        public string? TransactionNo { get; set; }


        public bool IsRefund { get; set; } = false;


        public decimal RefundAmount { get; set; } = 0;


        public DateTime PaymentDate { get; set; } = DateTime.Now;


        public int? ReceivedBy { get; set; }


        public string? Remarks { get; set; }


        // Navigation
        public Billing Billing { get; set; } = null!;
    }
}
