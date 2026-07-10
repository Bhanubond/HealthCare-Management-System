using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Entities.BillingDetails
{
    [Table("BillQueueDetails")]
    public class BillQueueDetails
    {
        [Key]
        public int BillQueueId { get; set; }

        public int CaseSheetId { get; set; }
        public int PatientId { get; set; }

        public int PatientTreatmentId { get; set; }

        public int DeptId { get; set; }
        public int ServiceID { get; set; }

        public int Quantity { get; set; } = 1;

        public decimal Amount { get; set; }

        public bool IsProcessed { get; set; } = false;

        public int? ProcessedBillId { get; set; }
        public DateTime? ProcessedDate { get; set; }

        public bool IsCancelled { get; set; } = false;

        public string? CancelReason { get; set; }

        public int? CancelledBy { get; set; }
        public DateTime? CancelledDate { get; set; }


        public int? CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;


        // Navigation
        [ForeignKey(nameof(PatientTreatmentId))]
        public PatientTreatment? PatientTreatment { get; set; }
        public Billing? ProcessedBill { get; set; }
    }
}
