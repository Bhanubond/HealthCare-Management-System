using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Entities
{
    [Table("MASCourse")]
    public class MASCourse
    {
        [Key]
        public int CourseId { get; set; }
        [Display(Name = "Course Name")]
        public string CourseName { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        public ICollection<MASCourseYear>? CourseYears { get; set; }
    }
}
