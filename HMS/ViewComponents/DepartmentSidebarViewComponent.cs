using HMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HMS.ViewComponents
{
    public class DepartmentSidebarViewComponent : ViewComponent
    {
        private readonly IDashboardService _dashboardService;

        public DepartmentSidebarViewComponent(
            IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            int? selectedDepartmentId = null)
        {
            var selectedDeptId = HttpContext.Session.GetInt32("DeptId");
            var departments = await _dashboardService.GetDepartmentsAsync();

            if (selectedDepartmentId.HasValue)
            {
                foreach (var department in departments)
                {
                    department.IsSelected = department.DeptId == selectedDepartmentId.Value;
                }
            }

            return View(
                "~/Views/Shared/Components/DepartmentSidebar/Default.cshtml",
                departments);
        }
    }
}
