using HMS.Common;
using HMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HMS.Controllers
{
    public class TreatmentController : Controller
    {
        private readonly ITreatmentService _treatmentService;

        public TreatmentController(ITreatmentService treatmentService)
        {
            _treatmentService = treatmentService;
        }

        public IActionResult Index(int deptId)
        {
            if (deptId == 0) deptId = HttpContext.Session.GetInt32("DeptId") ?? 0;

            if (deptId == 0) return BadRequest("Department not selected.");

            ViewBag.DeptId = deptId;
            ViewBag.FromDate = DateTime.Today;
            ViewBag.ToDate = DateTime.Today;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetTreatmentQueue(int deptId, string fromDate, string toDate)
        {
            var from = DateTime.Parse(fromDate).Date;
            var to = DateTime.Parse(toDate).Date;

            var data = await _treatmentService.GetTreatmentQueue(deptId, from, to);

            return Json(data);
        }

        public async Task<IActionResult> TreatmentDetails(int deptId, int patientId)
        {
            if (deptId == 0 || patientId == 0)
                return BadRequest("Invalid request");

            var model = await _treatmentService.GetTreatmentScreenAsync(deptId, patientId);

            if (model == null)
                return NotFound();

            var viewName = deptId switch
            {
                (int)Department.GEN => "~/Views/Treatment/GeneralMedicine/TreatmentDetails.cshtml", 
                (int)Department.EMR => "~/Views/Treatment/Emergency/TreatmentDetails.cshtml", 
                (int)Department.PED => "~/Views/Treatment/Pediatrics/TreatmentDetails.cshtml", 
                
                _ => "~/Views/Treatment/Default/TreatmentDetails.cshtml"
            };

            return View(viewName, model);
        }
    }
}