using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HMS.Models
{
    public class CourseIndexDto { public int CourseId { get; set; } public string CourseName { get; set; } = string.Empty; public bool IsActive { get; set; } }
    public class CourseDetailsDto { public int CourseId { get; set; } public string CourseName { get; set; } = string.Empty; public bool IsActive { get; set; } }
    public class CourseCreateViewModel { [Required, Display(Name = "Course Name")] public string CourseName { get; set; } = string.Empty; [Display(Name = "Active")] public bool IsActive { get; set; } = true; }
    public class CourseEditViewModel : CourseCreateViewModel { public int CourseId { get; set; } }

    public class CourseYearIndexDto { public int CourseYearId { get; set; } public int CourseId { get; set; } public string CourseName { get; set; } = string.Empty; public string YearName { get; set; } = string.Empty; public bool IsActive { get; set; } }
    public class CourseYearDetailsDto : CourseYearIndexDto { }
    public class CourseYearListViewModel { public int? CourseId { get; set; } public List<SelectListItem> Courses { get; set; } = new(); public List<CourseYearIndexDto> CourseYears { get; set; } = new(); }
    public class CourseYearCreateViewModel { [Required, Display(Name = "Course")] public int? CourseId { get; set; } [Required, Display(Name = "Year")] public string YearName { get; set; } = string.Empty; [Display(Name = "Active")] public bool IsActive { get; set; } = true; public List<SelectListItem> Courses { get; set; } = new(); }
    public class CourseYearEditViewModel : CourseYearCreateViewModel { public int CourseYearId { get; set; } }

    public class DepartmentIndexDto { public int DeptId { get; set; } public string DeptCode { get; set; } = string.Empty; public string DeptName { get; set; } = string.Empty; public string DeptDisplayName { get; set; } = string.Empty; public int? DisplayOrder { get; set; } public bool IsActive { get; set; } }
    public class DepartmentDetailsDto : DepartmentIndexDto { public bool IsReferredView { get; set; } public bool IsService { get; set; } public bool IsUserRights { get; set; } public bool LoadUserDropdown { get; set; } public bool DelInd { get; set; } public string CssClass { get; set; } = string.Empty; public bool ClinicalDept { get; set; } public string TreeDeptName { get; set; } = string.Empty; }
    public class DepartmentCreateViewModel { [Required, Display(Name = "Department Code")] public string DeptCode { get; set; } = string.Empty; [Required, Display(Name = "Department Name")] public string DeptName { get; set; } = string.Empty; [Display(Name = "Display Name")] public string DeptDisplayName { get; set; } = string.Empty; public string CssClass { get; set; } = string.Empty; public int? DisplayOrder { get; set; } public string TreeDeptName { get; set; } = string.Empty; public bool IsReferredView { get; set; } public bool IsService { get; set; } public bool IsUserRights { get; set; } public bool LoadUserDropdown { get; set; } public bool DelInd { get; set; } public bool ClinicalDept { get; set; } [Display(Name = "Active")] public bool IsActive { get; set; } = true; }
    public class DepartmentEditViewModel : DepartmentCreateViewModel { public int DeptId { get; set; } }

    public class DesignationIndexDto { public int DesignationId { get; set; } public string DesignationName { get; set; } = string.Empty; public bool IsActive { get; set; } }
    public class DesignationDetailsDto : DesignationIndexDto { }
    public class DesignationCreateViewModel { [Required, Display(Name = "Designation Name")] public string DesignationName { get; set; } = string.Empty; [Display(Name = "Active")] public bool IsActive { get; set; } = true; }
    public class DesignationEditViewModel : DesignationCreateViewModel { public int DesignationId { get; set; } }

    public class CountryIndexDto { public int CountryId { get; set; } public string CountryName { get; set; } = string.Empty; public bool IsActive { get; set; } public bool IsDeleted { get; set; } }
    public class CountryDetailsDto : CountryIndexDto { }
    public class CountryCreateViewModel { [Required, Display(Name = "Country Name")] public string CountryName { get; set; } = string.Empty; [Display(Name = "Active")] public bool IsActive { get; set; } = true; public bool IsDeleted { get; set; } }
    public class CountryEditViewModel : CountryCreateViewModel { public int CountryId { get; set; } }

    public class StateIndexDto { public int StateId { get; set; } public string CountryName { get; set; } = string.Empty; public string StateName { get; set; } = string.Empty; public bool IsActive { get; set; } public bool IsDeleted { get; set; } }
    public class StateDetailsDto : StateIndexDto { public int CountryId { get; set; } }
    public class StateCreateViewModel { [Required, Display(Name = "Country")] public int? CountryId { get; set; } [Required, Display(Name = "State Name")] public string StateName { get; set; } = string.Empty; [Display(Name = "Active")] public bool IsActive { get; set; } = true; public bool IsDeleted { get; set; } public List<SelectListItem> Countries { get; set; } = new(); }
    public class StateEditViewModel : StateCreateViewModel { public int StateId { get; set; } }

    public class MandalIndexDto { public int MandalId { get; set; } public string StateName { get; set; } = string.Empty; public string MandalName { get; set; } = string.Empty; public bool IsActive { get; set; } public bool IsDeleted { get; set; } }
    public class MandalDetailsDto : MandalIndexDto { public int StateId { get; set; } }
    public class MandalCreateViewModel { [Required, Display(Name = "State")] public int? StateId { get; set; } [Required, Display(Name = "Mandal Name")] public string MandalName { get; set; } = string.Empty; [Display(Name = "Active")] public bool IsActive { get; set; } = true; public bool IsDeleted { get; set; } public List<SelectListItem> States { get; set; } = new(); }
    public class MandalEditViewModel : MandalCreateViewModel { public int MandalId { get; set; } }

    public class CityIndexDto { public int CityId { get; set; } public string MandalName { get; set; } = string.Empty; public string CityName { get; set; } = string.Empty; public bool IsActive { get; set; } public bool IsDeleted { get; set; } }
    public class CityDetailsDto : CityIndexDto { public int MandalId { get; set; } }
    public class CityCreateViewModel { [Required, Display(Name = "Mandal")] public int? MandalId { get; set; } [Required, Display(Name = "City Name")] public string CityName { get; set; } = string.Empty; [Display(Name = "Active")] public bool IsActive { get; set; } = true; public bool IsDeleted { get; set; } public List<SelectListItem> Mandals { get; set; } = new(); }
    public class CityEditViewModel : CityCreateViewModel { public int CityId { get; set; } }

    public class ScreenIndexDto { public int ScreenId { get; set; } public string ScreenName { get; set; } = string.Empty; public string ScreenDisplayName { get; set; } = string.Empty; public string ControllerName { get; set; } = string.Empty; public string ActionName { get; set; } = string.Empty; public int? OrderDisplay { get; set; } public bool IsActive { get; set; } }
    public class ScreenDetailsDto : ScreenIndexDto { public string Title { get; set; } = string.Empty; public bool DelInd { get; set; } }
    public class ScreenCreateViewModel { [Required, Display(Name = "Screen Name")] public string ScreenName { get; set; } = string.Empty; [Required, Display(Name = "Display Name")] public string ScreenDisplayName { get; set; } = string.Empty; public string Title { get; set; } = string.Empty; public string ControllerName { get; set; } = string.Empty; public string ActionName { get; set; } = string.Empty; public bool DelInd { get; set; } public int? OrderDisplay { get; set; } [Display(Name = "Active")] public bool IsActive { get; set; } = true; }
    public class ScreenEditViewModel : ScreenCreateViewModel { public int ScreenId { get; set; } }

    public class DeptScreenMappingIndexDto { public int DeptScreenId { get; set; } public string DepartmentName { get; set; } = string.Empty; public string ScreenName { get; set; } = string.Empty; public bool CanCreate { get; set; } public bool CanRead { get; set; } public bool CanUpdate { get; set; } public bool CanDelete { get; set; } public bool IsActive { get; set; } }
    public class DeptScreenMappingDetailsDto : DeptScreenMappingIndexDto { public int DeptId { get; set; } public int ScreenId { get; set; } }
    public class DeptScreenMappingCreateViewModel { [Required, Display(Name = "Department")] public int? DeptId { get; set; } [Required, Display(Name = "Screen")] public int? ScreenId { get; set; } public bool CanCreate { get; set; } public bool CanRead { get; set; } = true; public bool CanUpdate { get; set; } public bool CanDelete { get; set; } [Display(Name = "Active")] public bool IsActive { get; set; } = true; public List<SelectListItem> Departments { get; set; } = new(); public List<SelectListItem> Screens { get; set; } = new(); }
    public class DeptScreenMappingEditViewModel : DeptScreenMappingCreateViewModel { public int DeptScreenId { get; set; } }
}
