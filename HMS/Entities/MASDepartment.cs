using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Entities
{
    [Table("MASDepartment")]
    public class MASDepartment
    {
        [Key]
        public int DeptId { get; set; }
        public string DeptCode { get; set; }
        public string DeptName { get; set; }

        public bool IsReferredView { get; set; }
        public bool IsService { get; set; }
        public bool IsUserRights { get; set; }
        public bool LoadUserDropdown { get; set; }

        public string DeptDisplayName { get; set; }
        public bool DelInd { get; set; }

        public string CssClass { get; set; }
        public int? DisplayOrder { get; set; }

        public bool ClinicalDept { get; set; }
        public string TreeDeptName { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        public ICollection<MASDeptScreenMapping>? ScreenMappings { get; set; }
    }
}
