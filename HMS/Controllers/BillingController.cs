using HMS.Models;
using HMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HMS.Controllers
{
    public class BillingController : Controller
    {
        private readonly IBillingService _billingService;

        public BillingController(IBillingService billingService)
        {
            _billingService = billingService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var bills = await _billingService.GetUnpaidBills();
            return View(bills);
        }

        [HttpGet]
        public async Task<IActionResult> PaidBills(int? department, string? patient, string? billNo, DateTime? billDate)
        {
            var bills = await _billingService.GetPaidBills(department, patient, billNo, billDate);
            ViewBag.Department = department;
            ViewBag.Patient = patient;
            ViewBag.BillNo = billNo;
            ViewBag.BillDate = billDate?.ToString("yyyy-MM-dd");
            return View(bills);
        }

        [HttpGet]
        public async Task<IActionResult> ViewBill(int id)
        {
            var model = await _billingService.GetBillDetails(id);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> View(int? id, int patientId = 0, int caseSheetId = 0, int deptId = 0)
        {
            if ((id ?? 0) > 0 && patientId == 0 && caseSheetId == 0 && deptId == 0)
            {
                return await ViewBill(id!.Value);
            }

            if (patientId <= 0 || caseSheetId <= 0 || deptId <= 0)
                return BadRequest("Invalid pending bill request.");

            var model = await _billingService.GetPendingBillDetails(patientId, caseSheetId, deptId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PayBill(BillingPaymentVm model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var billId = await _billingService.SavePayment(model, userId);

            TempData["SuccessMessage"] = "Payment saved successfully.";
            return RedirectToAction(nameof(ViewBill), new { id = billId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelBill(BillingCancelVm model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            await _billingService.CancelBill(model, userId);

            TempData["SuccessMessage"] = "Bill cancelled successfully.";
            return RedirectToAction(nameof(PaidBills));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelService(BillingServiceCancelVm model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            await _billingService.CancelService(model, userId);

            TempData["SuccessMessage"] = "Service cancelled successfully.";
            return RedirectToAction(nameof(ViewBill), new { id = model.BillId });
        }
    }
}
