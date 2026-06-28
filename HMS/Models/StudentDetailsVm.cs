namespace HMS.Models
{
    public class StudentDetailsVm
    {
        public long StudentId { get; set; }

        public string StudentName { get; set; }

        public string? StudentCode { get; set; }
        public string? Gender { get; set; }
        public DateTime? DOB { get; set; }

        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }

        public DateTime AdmissionDate { get; set; }
        public bool IsActive { get; set; }

        // Related Names
        public string? CourseName { get; set; }
        public string? YearName { get; set; }
        public string? DeptName { get; set; }

        public string? CountryName { get; set; }
        public string? StateName { get; set; }
        public string? MandalName { get; set; }
        public string? CityName { get; set; }
    }
}