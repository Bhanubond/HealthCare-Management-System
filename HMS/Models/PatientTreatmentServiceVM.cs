namespace HMS.Models
{
    public class PatientTreatmentServiceVM
    {

        public int ServiceID { get; set; }

        public string ServiceName { get; set; }


        public decimal Rate { get; set; }


        public int Quantity { get; set; } = 1;


        public decimal DiscountPer { get; set; }


        public decimal Amount { get; set; }


        public bool Selected { get; set; }

    }
}
