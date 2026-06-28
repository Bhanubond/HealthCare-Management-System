using HMS.Entities;

namespace HMS.Models
{
    public class StudentFormViewModel
    {
        public Student Student { get; set; } = new Student();
        public List<MASCourse> Courses { get; set; } = new();
        public List<MASCourseYear> CourseYears { get; set; } = new();
        public List<MASDepartment> Departments { get; set; } = new();
        public List<MASCountry> Countries { get; set; } = new();
        public List<MASState> States { get; set; } = new();
        public List<MASMandal> Mandals { get; set; } = new();
        public List<MASCity> Cities { get; set; } = new();
    }
}
