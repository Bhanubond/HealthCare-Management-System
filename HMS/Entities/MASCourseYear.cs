using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Entities
{
    [Table("MASCourseYear")]
    public class MASCourseYear
    {
        [Key]
        public int CourseYearId { get; set; }
        [Display(Name = "Course")]
        public int CourseId { get; set; }
        [Display(Name = "Year")]
        public string YearName { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        [Display(Name = "Course")]
        public MASCourse? Course { get; set; }
    }
}
