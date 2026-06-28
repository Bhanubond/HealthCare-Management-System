using HMS.Entities;
using HMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HMS.Controllers
{
    public class MASDepartmentController : BaseController
    {
        private readonly IMASDepartmentService _service;

        public MASDepartmentController(IMASDepartmentService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {

            return View(await _service.GetAllAsync());
        }

        [HttpGet]
        public IActionResult Create()
        {

            return View(new MASDepartment());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MASDepartment model)
        {

            if (!ModelState.IsValid)
            {
                Error("Please correct the highlighted fields and try again.");
                return View(model);
            }

            await _service.CreateAsync(model);
            Success("Department created successfully.");
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MASDepartment model)
        {

            if (!ModelState.IsValid)
            {
                Error("Please correct the highlighted fields and try again.");
                return View(model);
            }

            await _service.UpdateAsync(model);
            Success("Department updated successfully.");
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(MASDepartment model)
        {
            if (await _service.DeleteAsync(model.DeptId))
            {
                Success("Department deleted successfully.");
            }
            else
            {
                Error("Department record was not found.");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
