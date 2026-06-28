using HMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HMS.ViewComponents
{
    public class ScreenCardsViewComponent : ViewComponent
    {
        private readonly IDashboardService _dashboardService;

        public ScreenCardsViewComponent(
            IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            int? departmentId = null,
            int? selectedScreenId = null)
        {
            var deptId = departmentId ?? HttpContext.Session.GetInt32("DeptId");
            var screens = deptId.HasValue
                ? await _dashboardService.GetScreensAsync(deptId.Value)
                : new List<HMS.Models.DashboardScreenCardViewModel>();

            if (selectedScreenId.HasValue)
            {
                foreach (var screen in screens)
                {
                    screen.IsSelected = screen.ScreenId == selectedScreenId.Value;
                }
            }

            return View(
                "~/Views/Shared/Components/ScreenCards/Default.cshtml",
                screens);
        }
    }
}
