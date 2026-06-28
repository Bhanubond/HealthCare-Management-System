using HMS.Models;
using HMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HMS.Controllers
{
    public class UsersController : BaseController
    {
        private readonly IUserService _service;

        public UsersController(IUserService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _service.GetAllAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View(await _service.BuildCreateAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                Error("Please correct the highlighted fields and try again.");
                return View(await _service.BuildFormAsync(vm));
            }

            await _service.CreateAsync(vm);
            Success("User created successfully.");
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var vm = await _service.BuildEditAsync(id);
            if (vm == null)
            {
                return NotFound();
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                Error("Please correct the highlighted fields and try again.");
                return View(await _service.BuildFormAsync(vm));
            }

            if (!await _service.UpdateAsync(vm))
            {
                return NotFound();
            }

            Success("User updated successfully.");
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var item = await _service.GetDetailsAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _service.GetForDeleteAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(UserDetailsDto model)
        {
            if (await _service.DeleteAsync(model.UserId))
            {
                Success("User deleted successfully.");
            }
            else
            {
                Error("User record was not found.");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
