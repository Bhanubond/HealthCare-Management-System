namespace HMS.Models
{
    public class OPDSearchResultVm
    {
        public int PatientId { get; set; }
        public string UHID { get; set; }
        public string OpNo { get; set; }
        public string PatientName { get; set; }
        public string Phone { get; set; }
        public string AadharNo { get; set; }
        public DateTime RegDate { get; set; }

        public int TotalRecords { get; set; }
    }
}
