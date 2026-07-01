using HMS.Models;
using HMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HMS.Controllers
{
    public class GeneralMedicineController : Controller
    {
        private readonly IAllotmentService _allotmentService;
        private readonly IGeneralMedicineServices _generalMedicineServices;

        public GeneralMedicineController(IAllotmentService allotmentService, IGeneralMedicineServices generalMedicineServices)
        {
            _allotmentService = allotmentService;
            _generalMedicineServices = generalMedicineServices;
        }

        public IActionResult Index() => View();

        public async Task<IActionResult> Allotment(int deptId)
        {
            if (deptId == 0)
                deptId = HttpContext.Session.GetInt32("DeptId") ?? 0;

            if (deptId == 0)
                return BadRequest("Department not selected");

            var data = await _allotmentService.GetPatientsByDepartment(deptId);
            ViewBag.DeptId = deptId;
            return View("~/Views/Allotment/Index.cshtml", data);
        }

        public async Task<IActionResult> PatientSearch()
            => View(await _generalMedicineServices.GetCompletedCases());

        [HttpGet]
        public async Task<IActionResult> EditCaseSheet(int gmId)
        {
            var model = await _generalMedicineServices.GetCaseSheetById(gmId);
            return model.GMID == 0 ? NotFound() : View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditCaseSheet(GMCasesheetSaveVm model)
        {
            await _generalMedicineServices.UpdateCaseSheet(model);
            return RedirectToAction(nameof(PatientSearch));
        }

        public async Task<IActionResult> Treatment()
            => View(await _generalMedicineServices.GetPendingTreatmentPatients());

        public async Task<IActionResult> TreatmentDetails(int patientId)
        {
            var model = await _generalMedicineServices.GetTreatmentScreenAsync(patientId);
            return model.PatientId == 0 ? NotFound() : View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveCaseSheet(GMCasesheetSaveVm model)
        {
            await _generalMedicineServices.SaveCaseSheet(model);
            return RedirectToAction(nameof(Treatment));
        }

        public IActionResult Approvals() => View();
    }
}
