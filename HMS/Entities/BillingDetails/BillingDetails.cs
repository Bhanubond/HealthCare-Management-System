using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Entities.BillingDetails
{
    [Table("BillingDetails")]
    public class BillingDetails
    {
        [Key]
        public int BillDetailId { get; set; }

        public int BillId { get; set; }

        public int PatientTreatmentId { get; set; }

        public int ServiceID { get; set; }


        public int Quantity { get; set; } = 1;

        public int CancelledQty { get; set; } = 0;


        public decimal Rate { get; set; }

        public decimal DiscountPer { get; set; } = 0;


        public decimal Amount { get; set; }


        public bool IsCancelled { get; set; } = false;


        public string? CancelReason { get; set; }

        public int? CancelledBy { get; set; }

        public DateTime? CancelledDate { get; set; }


        public DateTime CreatedDate { get; set; } = DateTime.Now;


        // Navigation to Billing Header

        [ForeignKey(nameof(BillId))]
        public Billing? Billing { get; set; }



        // Navigation to Patient Treatment

        [ForeignKey(nameof(PatientTreatmentId))]
        public PatientTreatment? PatientTreatment { get; set; }



        // Navigation to Service Master

        [ForeignKey(nameof(ServiceID))]
        public TreatmentServices? Service { get; set; }
    }
}
