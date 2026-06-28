using HMS.Entities;
using HMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HMS.Controllers
{
    public class MASScreenController : BaseController
    {
        private readonly IScreenService _service;

        public MASScreenController(IScreenService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            ConfigureList("Screens");
            return View(await _service.GetAllAsync());
        }

        [HttpGet]
        public IActionResult Create()
        {
            ConfigureForm("Create Screen", "Create", "Save");
            return View(new MASScreen { IsActive = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MASScreen model)
        {
            ConfigureForm("Create Screen", "Create", "Save");
            if (!ModelState.IsValid)
            {
                Error("Please correct the highlighted fields and try again.");
                return View(model);
            }

            await _service.CreateAsync(model);
            Success("Screen created successfully.");
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

            ConfigureForm("Edit Screen", "Edit", "Update");
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MASScreen model)
        {
            ConfigureForm("Edit Screen", "Edit", "Update");
            if (!ModelState.IsValid)
            {
                Error("Please correct the highlighted fields and try again.");
                return View(model);
            }

            await _service.UpdateAsync(model);
            Success("Screen updated successfully.");
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

            ConfigureDetails("Screen Details");
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

            ConfigureDelete("Delete Screen");
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(MASScreen model)
        {
            if (await _service.DeleteAsync(model.ScreenId))
            {
                Success("Screen deleted successfully.");
            }
            else
            {
                Error("Screen record was not found.");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
