using HMS.Models;
using HMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HMS.Controllers
{
    public class PediatricsController : Controller
    {
        private readonly IAllotmentService _allotmentService;
        private readonly IPediatricsServices _pediatricsServices;
        private readonly ITreatmentService _treatmentService;

        public PediatricsController(IAllotmentService allotmentService, IPediatricsServices pediatricsServices, ITreatmentService treatmentService)
        {
            _allotmentService = allotmentService;
            _pediatricsServices = pediatricsServices;
            _treatmentService = treatmentService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SaveCaseSheet([FromBody] PedoCasesheetScreenVm model)
        {
            if (model == null)
                return BadRequest("Model is null");

            await _pediatricsServices.SaveCaseSheet(model);

            return Json(new { success = true });
        }

        public IActionResult PatientSearch()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetCompletedCases(DateTime fromDate, DateTime toDate)
        {
            var data = await _pediatricsServices.GetCompletedCases(fromDate, toDate);
            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> EditCaseSheet(int PedoID)
        {
            var model = await _pediatricsServices.GetCaseSheetById(PedoID);
            return model.PedoID == 0 ? NotFound() : View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditCaseSheet(PedoCasesheetScreenVm model)
        {
            await _pediatricsServices.UpdateCaseSheet(model);
            return RedirectToAction(nameof(PatientSearch));
        }

        public IActionResult Approvals() => View();

        [HttpGet]
        public async Task<IActionResult> GetApprovalQueue(DateTime? fromDate, DateTime? toDate)
        {
            fromDate ??= DateTime.Now.AddMonths(-1);
            toDate ??= DateTime.Now;

            var data = await _pediatricsServices.GetApprovalQueue(fromDate.Value, toDate.Value);

            return Json(data);
        }


        [HttpPost]
        public async Task<IActionResult> SendForApproval(int PedoID)
        {
            try
            {
                var message = await _pediatricsServices.ProcessApprovalFlow(PedoID);

                return Json(new
                {
                    success = true,
                    message = message
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}
