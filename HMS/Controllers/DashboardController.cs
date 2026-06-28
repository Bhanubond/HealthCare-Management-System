using HMS.Models;
using HMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HMS.Controllers
{
    public class DashboardController : BaseController
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? deptId = null)
        {
            var departments = await _dashboardService.GetDepartmentsAsync();
            var selectedDepartmentId = deptId ?? HttpContext.Session.GetInt32("DeptId");
            string selectedDepartmentName = string.Empty;

            if (selectedDepartmentId.HasValue)
            {
                HttpContext.Session.SetInt32("DeptId", selectedDepartmentId.Value);
                selectedDepartmentName = departments
                    .FirstOrDefault(x => x.DeptId == selectedDepartmentId.Value)
                    ?.DeptName ?? string.Empty;
            }

            return View(new DashboardViewModel
            {
                Departments = departments,
                SelectedDepartmentId = selectedDepartmentId,
                SelectedDepartmentName = selectedDepartmentName
            });
        }

        [HttpGet]
        public IActionResult SelectDepartment(int deptId)
        {
            HttpContext.Session.SetInt32("DeptId", deptId);
            return RedirectToAction(nameof(Index), new { deptId });
        }
    }
}
