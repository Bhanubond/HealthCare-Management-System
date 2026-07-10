namespace HMS.Models
{
    public class PendingBillQueueVm
    {
        public int BillQueueId { get; set; }
        public int CaseSheetId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }

        public int PatientTreatmentId { get; set; }

        public int DeptId { get; set; }
        public string DeptName { get; set; }

        public int ServiceID { get; set; }
        public string ServiceName { get; set; }

        public decimal Quantity { get; set; }
        public decimal Amount { get; set; }

        public bool IsProcessed { get; set; }
        public int? ProcessedBillId { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
