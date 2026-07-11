using HMS.Entities.BillingDetails;

namespace HMS.Models
{
    public class BillingListItemVm
    {
        public int BillId { get; set; }
        public string BillNo { get; set; } = string.Empty;
        public int CaseSheetId { get; set; }
        public int PatientId { get; set; }
        public int DeptId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string OpNo { get; set; } = string.Empty;
        public string DeptName { get; set; } = string.Empty;
        public int PendingServiceCount { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal BalanceAmount { get; set; }
        public decimal RefundAmount { get; set; }
        public bool IsPaid { get; set; }
        public bool IsCancelled { get; set; }
        public DateTime BillDate { get; set; }
        public int BillQueueId { get;  set; }
    }

    public class BillingQueueItemVm
    {
        public int BillQueueId { get; set; }
        public int PatientTreatmentId { get; set; }
        public int ServiceID { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Rate { get; set; }
        public decimal DiscountPer { get; set; }
        public decimal Amount { get; set; }
        public bool IsProcessed { get; set; }
        public bool IsCancelled { get; set; }
    }

    public class BillingPaymentVm
    {
        public int BillId { get; set; }
        public int PatientId { get; set; }
        public int CaseSheetId { get; set; }
        public int DeptId { get; set; }
        public string PaymentMode { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? TransactionNo { get; set; }
        public string? Remarks { get; set; }
        public List<BillingPaymentLineVm> Payments { get; set; } = new();
    }

    public class BillingPaymentLineVm
    {
        public string PaymentMode { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? TransactionNo { get; set; }
        public string? Remarks { get; set; }
    }

    public class BillingCancelVm
    {
        public int BillId { get; set; }
        public string? CancelReason { get; set; }
    }

    public class BillingServiceCancelVm
    {
        public int BillId { get; set; }
        public int BillDetailId { get; set; }
        public string? CancelReason { get; set; }
    }

    public class BillingViewVm
    {
        public Billing? Billing { get; set; }
        public PatientDetailsViewModel? Patient { get; set; }
        public List<BillingQueueItemVm> PendingItems { get; set; } = new();
        public List<BillingDetails> BillingDetails { get; set; } = new();
        public List<PaymentDetail> Payments { get; set; } = new();
        public int CaseSheetId { get; set; }
        public int PatientId { get; set; }
        public int DeptId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
    }
}
