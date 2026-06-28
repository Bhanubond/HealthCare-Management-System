using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Entities
{
    [Table("MASDeptScreenMapping")]
    public class MASDeptScreenMapping
    {
        [Key]
        public int DeptScreenId { get; set; }

        public int DeptId { get; set; }
        public int ScreenId { get; set; }

        public bool CanCreate { get; set; }
        public bool CanRead { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        public MASDepartment? Department { get; set; }
        public MASScreen? Screen { get; set; }
    }
}
