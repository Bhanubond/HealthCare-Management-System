namespace HMS.Models
{
    public class PatientServiceVM
    {
        public int ServiceID { get; set; }

        public string ServiceName { get; set; } = string.Empty;

        public decimal Rate { get; set; }

        public decimal Quantity { get; set; } = 1;

        public decimal DiscountPer { get; set; } = 0;

        public decimal Amount { get; set; }

        public bool Selected { get; set; }
    }
}
