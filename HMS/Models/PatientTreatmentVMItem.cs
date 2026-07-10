namespace HMS.Models
{
    public class PatientTreatmentVMItem
    {
        public int TreatmentId { get; set; }

        public int ServiceID { get; set; }

        public string ServiceName { get; set; } = string.Empty;

        public decimal Rate { get; set; }

        public decimal Quantity { get; set; }

        public decimal DiscountPer { get; set; }

        public decimal Amount { get; set; }
    }
}
