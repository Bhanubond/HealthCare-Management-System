namespace HMS.Models
{
    public class DashboardScreenCardViewModel
    {
        public int ScreenId { get; set; }
        public string ScreenName { get; set; } = string.Empty;
        public string ScreenDisplayName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string ControllerName { get; set; } = string.Empty;
        public string ActionName { get; set; } = string.Empty;
        public string IconClass { get; set; } = "bi-grid";
        public bool CanCreate { get; set; }
        public bool CanRead { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
        public bool IsSelected { get; set; }
    }
}
