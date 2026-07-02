namespace HMS.Models
{
    public class TreatmentQueueVm
    {
        public int PatientId { get; set; }

        public string OpNo { get; set; }

        public string PatientName { get; set; }

        public string CategoryName { get; set; }

        public string DepartmentName { get; set; }

        public string DoctorName { get; set; }

        public string StudentName { get; set; }

        public DateTime? AllotDate { get; set; }

        public int DeptId { get; set; }
    }
}
