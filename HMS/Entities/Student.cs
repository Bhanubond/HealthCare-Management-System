using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Entities
{
    [Table("Students")]
    public class Student
    {
        [Key]
        public long StudentId { get; set; }

        public string? StudentCode { get; set; }

        [Required]
        public string StudentName { get; set; } = string.Empty;

        public string? Gender { get; set; }

        public DateTime? DOB { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }

        public int? CourseId { get; set; }
        public int? CourseYearId { get; set; }
        public int? DepartmentId { get; set; }

        public int? CountryId { get; set; }
        public int? StateId { get; set; }
        public int? MandalId { get; set; }
        public int? CityId { get; set; }

        public DateTime AdmissionDate { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        public MASCourse? Course { get; set; }
        public MASCourseYear? CourseYear { get; set; }
        public MASDepartment? Department { get; set; }

        public MASCountry? Country { get; set; }
        public MASState? State { get; set; }
        public MASMandal? Mandal { get; set; }
        public MASCity? City { get; set; }
    }
}