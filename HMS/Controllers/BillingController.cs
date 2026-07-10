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
        public async Task<IActionResult> PaidBills()
        {
            var bills = await _billingService.GetPaidBills();
            return View(bills);
        }

        [HttpGet]
        public async Task<IActionResult> ViewBill(int id)
        {
            var model = await _billingService.GetBillDetailsByCaseSheetId(id);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> View(int patientId)
        {
            var pendingItems = await _billingService.GetPendingBillQueueByPatientId(patientId);

            return View(pendingItems);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBill(int caseSheetId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var billId = await _billingService.CreateBill(caseSheetId, userId);

            if (billId == 0)
            {
                TempData["ErrorMessage"] = "No pending services were found for billing.";
                return RedirectToAction(nameof(ViewBill), new { id = caseSheetId });
            }

            TempData["SuccessMessage"] = "Bill generated successfully.";
            return RedirectToAction(nameof(View), new { id = billId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PayBill(BillingPaymentVm model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            await _billingService.SavePayment(model, userId);

            TempData["SuccessMessage"] = "Payment saved successfully.";
            return RedirectToAction(nameof(View), new { id = model.BillId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelBill(BillingCancelVm model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            await _billingService.CancelBill(model, userId);

            TempData["SuccessMessage"] = "Bill cancelled successfully.";
            return RedirectToAction(nameof(View), new { id = model.BillId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelService(BillingServiceCancelVm model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            await _billingService.CancelService(model, userId);

            TempData["SuccessMessage"] = "Service cancelled successfully.";
            return RedirectToAction(nameof(View), new { id = model.BillId });
        }
    }
}
