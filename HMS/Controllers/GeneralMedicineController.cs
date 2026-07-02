using HMS.Models;
using HMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HMS.Controllers
{
    public class GeneralMedicineController : Controller
    {
        private readonly IAllotmentService _allotmentService;
        private readonly IGeneralMedicineServices _generalMedicineServices;
        private readonly ITreatmentService _treatmentService;

        public GeneralMedicineController(IAllotmentService allotmentService, IGeneralMedicineServices generalMedicineServices, ITreatmentService treatmentService)
        {
            _allotmentService = allotmentService;
            _generalMedicineServices = generalMedicineServices;
            _treatmentService = treatmentService;
        }

        public IActionResult Index() => View();

        //public async Task<IActionResult> PatientSearch()
        //    => View(await _generalMedicineServices.GetCompletedCases());
        public IActionResult PatientSearch()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetCompletedCases(DateTime fromDate, DateTime toDate)
        {
            var data = await _generalMedicineServices.GetCompletedCases(fromDate, toDate);
            return Json(data);
        }

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


        [HttpPost]
        public async Task<IActionResult> SaveCaseSheet(GMCasesheetSaveVm model)
        {
            await _generalMedicineServices.SaveCaseSheet(model);
            return RedirectToAction(nameof(PatientSearch));
        }

        public IActionResult Approvals() => View();
    }
}
