namespace HMS.Models
{
    public class TreatmentServiceItemVM
    {

        public int ServiceID { get; set; }


        public string? ServiceName { get; set; }


        public decimal Rate { get; set; }


        public int Quantity { get; set; } = 1;


        public decimal Amount { get; set; }
        public int DiscountPer { get; set; } = 0;


        public bool Selected { get; set; }

    }
}
