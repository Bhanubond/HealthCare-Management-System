using HMS.Models;
using HMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HMS.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _accountService.AuthenticateAsync(model);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid username or password");
                TempData["ErrorMessage"] = "Invalid username or password.";
                return View(model);
            }

            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserName", user.UserName);

            await _accountService.RecordLoginAsync(
                user,
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                Request.Headers["User-Agent"].ToString(),
                HttpContext.Session.Id);

            TempData["SuccessMessage"] = "Login successful.";
            return RedirectToAction("Index", "Dashboard");
        }

        public async Task<IActionResult> Logout()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId.HasValue)
            {
                await _accountService.RecordLogoutAsync(userId.Value);
            }

            HttpContext.Session.Clear();
            TempData["SuccessMessage"] = "You have been logged out.";

            return RedirectToAction(nameof(Login));
        }
    }
}
