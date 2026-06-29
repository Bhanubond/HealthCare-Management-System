using HMS.Entities;

namespace HMS.Models
{
    public class StudentAllotmentViewModel
    {
        public int PatientId { get; set; }

        public int DeptId { get; set; }

        public string PatientName { get; set; }

        public int? StudentId { get; set; }

        public int? DoctorId { get; set; }

        public List<Student> Students { get; set; } = new();

        public List<Doctor> Doctors { get; set; } = new();
        public int? ReferredId { get; set; }
    }
}
