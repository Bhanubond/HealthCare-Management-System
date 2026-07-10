using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Entities.BillingDetails
{
    [Table("PatientTreatments")]
    public class PatientTreatment
    {
        [Key]
        public int PatientTreatmentId { get; set; }


        public int CaseSheetId { get; set; }


        public int PatientId { get; set; }


        public int DeptId { get; set; }


        public int ServiceID { get; set; }


        public int? DoctorId { get; set; }


        public int Quantity { get; set; } = 1;


        [Column(TypeName = "decimal(10,2)")]
        public decimal Rate { get; set; }


        [Column(TypeName = "decimal(10,2)")]
        public decimal DiscountPer { get; set; }


        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }


        public bool IsBilled { get; set; } = false;


        public bool IsCancelled { get; set; } = false;


        public string? CancelReason { get; set; }


        public int? CancelledBy { get; set; }


        public DateTime? CancelledDate { get; set; }


        public DateTime TreatmentDate { get; set; }
            = DateTime.Now;


        public int? CreatedBy { get; set; }


        public DateTime CreatedDate { get; set; }
            = DateTime.Now;




        [ForeignKey(nameof(ServiceID))]
        public TreatmentServices? Service { get; set; }


        // Department Master

        [ForeignKey(nameof(DeptId))]
        public MASDepartment? Department { get; set; }



        // Billing Navigation

        public ICollection<BillQueueDetails> BillQueueDetails { get; set; }
            = new List<BillQueueDetails>();


        public ICollection<BillingDetails> BillingDetails { get; set; }
            = new List<BillingDetails>();

    }
}