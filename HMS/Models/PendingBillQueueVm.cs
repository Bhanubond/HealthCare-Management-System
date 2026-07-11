namespace HMS.Models
{
    public class PendingBillQueueVm
    {
        public int BillQueueId { get; set; }
        public int CaseSheetId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string OpNo { get; set; } = string.Empty;

        public int PatientTreatmentId { get; set; }

        public int DeptId { get; set; }
        public string DeptName { get; set; } = string.Empty;

        public int ServiceID { get; set; }
        public string ServiceName { get; set; } = string.Empty;

        public decimal Quantity { get; set; }
        public decimal Rate { get; set; }
        public decimal DiscountPer { get; set; }
        public decimal Amount { get; set; }

        public bool IsProcessed { get; set; }
        public int? ProcessedBillId { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
