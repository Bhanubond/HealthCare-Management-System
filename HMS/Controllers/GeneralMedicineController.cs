using HMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HMS.Controllers
{
    public class GeneralMedicineController : Controller
    {
        private readonly IAllotmentService _allotmentService;

        public GeneralMedicineController(IAllotmentService allotmentService)
        {
            _allotmentService = allotmentService;
        }

        // 🔹 Dashboard / Main Page
        public IActionResult Index()
        {
            return View();
        }

        // 🔹 Allotment Queue (Department-wise patients)
        public async Task<IActionResult> Allotment(int deptId)
        {
            if (deptId == 0)
            {
                deptId = HttpContext.Session.GetInt32("DeptId") ?? 0;
            }

            if (deptId == 0)
                return BadRequest("Department not selected");

            var data = await _allotmentService.GetPatientsByDepartment(deptId);

            ViewBag.DeptId = deptId;
            return View("~/Views/Allotment/Index.cshtml", data);
        }

        // 🔹 Patient Details Page
        public IActionResult PatientSearch()
        {
            return View();
        }

        // 🔹 Treatment Screen
        public IActionResult Treatment(int patientId)
        {
            ViewBag.PatientId = patientId;
            return View();
        }

        // 🔹 Approvals Screen
        public IActionResult Approvals()
        {
            return View();
        }
    }
}