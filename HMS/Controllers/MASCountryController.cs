using HMS.Entities;
using HMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace HMS.Controllers
{
    public class MASCountryController : BaseController
    {
        private readonly IMASCountryService _service;

        public MASCountryController(IMASCountryService service)
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
            return View(new MASCountry { IsActive = true, IsDeleted = false, IsDefault = false });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MASCountry model)
        {
            if (!ModelState.IsValid)
            {
                Error("Please correct the highlighted fields and try again.");
                return View(model);
            }

            await _service.CreateAsync(model);
            Success("Country created successfully.");
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
        public async Task<IActionResult> Edit(MASCountry model)
        {
            if (!ModelState.IsValid)
            {
                Error("Please correct the highlighted fields and try again.");
                return View(model);
            }

            await _service.UpdateAsync(model);
            Success("Country updated successfully.");
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
        public async Task<IActionResult> DeleteConfirmed(MASCountry model)
        {
            if (await _service.DeleteAsync(model.CountryId))
            {
                Success("Country deleted successfully.");
            }
            else
            {
                Error("Country record was not found.");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
