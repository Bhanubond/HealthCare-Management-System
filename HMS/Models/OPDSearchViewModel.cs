using HMS.Entities;

namespace HMS.Models
{
    public class OPDSearchViewModel
    {
        public string OPNo { get; set; }
        public string Phone { get; set; }
        public string PatientName { get; set; }
        public string AadharNo { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public int TotalRecords { get; set; }

        public List<OPDPatientRegistration> Results { get; set; }
    }
}
