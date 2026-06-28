using System.ComponentModel.DataAnnotations;

namespace HMS.Models
{
    public class CourseYearDto
    {
        [ScaffoldColumn(false)]
        public int CourseYearId { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string YearName { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
    }
}
