namespace HMS.Models
{
    public class DashboardDepartmentViewModel
    {
        public int DeptId { get; set; }
        public string DeptName { get; set; } = string.Empty;
        public string? TreeDeptName { get; set; }
        public string? CssClass { get; set; }
        public bool IsSelected { get; set; }
        public int ScreenCount { get; set; }
    }
}
