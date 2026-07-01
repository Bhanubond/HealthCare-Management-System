namespace HMS.Models
{
    public class GMCasesheetViewVm
    {
        public int PatientId { get; set; }
        public string? OpNo { get; set; }
        public string? PatientName { get; set; }
        public int Age { get; set; }
        public string? Gender { get; set; }
        public string? DoctorName { get; set; }
        public string? StudentName { get; set; }
        public int DoctorId { get; set; }
        public int StudentId { get; set; }
        public int? AllotId { get; set; }
        public int? ReferredId { get; set; }
    }
}
