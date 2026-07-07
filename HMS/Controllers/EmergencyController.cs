using HMS.Entities;
using HMS.Models;
using HMS.Services.Implementations;
using HMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HMS.Controllers
{
    public class EmergencyController : Controller
    {
        private readonly IAllotmentService _allotmentService;
        private readonly IEmergencyService _emergencyServices;
        private readonly ITreatmentService _treatmentService;

        public EmergencyController(IAllotmentService allotmentService, IEmergencyService emergencyServices, ITreatmentService treatmentService)
        {
            _allotmentService = allotmentService;
            _emergencyServices = emergencyServices;
            _treatmentService = treatmentService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SaveCaseSheet([FromBody] EMRCasesheetScreenVm model)
        {
            if (model == null)
                return BadRequest("Model is null");

            await _emergencyServices.SaveCaseSheet(model);

            return Json(new { success = true });
        }

        public IActionResult PatientSearch()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetCompletedCases(DateTime fromDate, DateTime toDate)
        {
            var data = await _emergencyServices.GetCompletedCases(fromDate, toDate);
            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> EditCaseSheet(int EMRId)
        {
            var model = await _emergencyServices.GetCaseSheetById(EMRId);
            return model.EMRId == 0 ? NotFound() : View(model);
        }

        //[HttpPost]
        //public async Task<IActionResult> EditCaseSheet(GMCasesheetSaveVm model)
        //{
        //    await _emergencyServices.UpdateCaseSheet(model);
        //    return RedirectToAction(nameof(PatientSearch));
        //}

    }

}
