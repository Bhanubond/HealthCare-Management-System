namespace HMS.Models
{
    public class DashboardViewModel
    {
        public List<DashboardDepartmentViewModel> Departments { get; set; } = new();
        public int? SelectedDepartmentId { get; set; }
        public string SelectedDepartmentName { get; set; } = string.Empty;
    }
}
