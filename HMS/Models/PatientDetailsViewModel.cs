namespace HMS.Models
{
    public class PatientDetailsViewModel
    {
        public string UHID { get; set; }
        public string OpNo { get; set; }
        public string PatientName { get; set; }
        public DateTime? Dob { get; set; }
        public int? Age { get; set; }

        public string Gender { get; set; }
        public string Phone { get; set; }

        public string CategoryName { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public string MandalName { get; set; }
        public string CityName { get; set; }
        public string Address { get; set; }
    }
}
