namespace HMS.Models
{
    public class TreatmentServiceListVM
    {
        public int ServiceID { get; set; }
        public string ServiceCode { get; set; }
        public string ServiceName { get; set; }
        public string DepartmentName { get; set; }
        public decimal Cost { get; set; }
        public bool IsActive { get; set; }
    }
}
