namespace HMS.Models
{
    public class GMCasesheetEditVm
    {
        public int GMID { get; set; }

        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public int StudentId { get; set; }
        public int? AllotId { get; set; }
        public int? ReferredId { get; set; }

        public string OpNo { get; set; }
        public string PatientName { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string DoctorName { get; set; }
        public string StudentName { get; set; }

        public string ChiefComplaint { get; set; }
        public string Symptoms { get; set; }
        public string Diagnosis { get; set; }
        public string Prescription { get; set; }
        public string Notes { get; set; }
    }
}
