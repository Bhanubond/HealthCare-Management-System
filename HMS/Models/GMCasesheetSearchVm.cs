namespace HMS.Models
{
    public class GMCasesheetSearchVm
    {
        public int GMID { get; set; }
        public int PatientId { get; set; }
        public string? PatientName { get; set; }
        public string? OpNo { get; set; }
        public string? Phone { get; set; }
        public string? Gender { get; set; }
        public int Age { get; set; }
        public string? DoctorName { get; set; }
        public string? StudentName { get; set; }
        public DateTime CaseDate { get; set; }
    }
}
