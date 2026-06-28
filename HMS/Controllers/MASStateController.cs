using HMS.Entities;
using HMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HMS.Controllers
{
    public class MASStateController : BaseController
    {
        private readonly IMASStateService _service;

        public MASStateController(IMASStateService service)
        {
            _service = service;
        }

        // ================= INDEX =================
        public async Task<IActionResult> Index(int? countryId)
        {
            var countries = await _service.GetActiveCountriesAsync();

            ViewBag.CountryId = new SelectList(
                countries,
                "CountryId",
                "CountryName",
                countryId
            );

            var data = await _service.GetAllAsync();

            if (countryId.HasValue)
            {
                data = data.Where(x => x.CountryId == countryId.Value).ToList();
            }

            return View(data);
        }

        // ================= CREATE =================
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadCountries();
            return View(new MASState { IsActive = true, IsDeleted = false });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MASState model)
        {
            await LoadCountries(model.CountryId);

            if (!ModelState.IsValid)
                return View(model);

            await _service.CreateAsync(model);
            return RedirectToAction(nameof(Index));
        }

        // ================= EDIT =================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();

            await LoadCountries(item.CountryId);
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MASState model)
        {
            await LoadCountries(model.CountryId);

            if (!ModelState.IsValid)
                return View(model);

            await _service.UpdateAsync(model);
            return RedirectToAction(nameof(Index));
        }

        // ================= DETAILS =================
        public async Task<IActionResult> Details(int id)
        {
            var item = await _service.GetByIdAsync(id);
            return item == null ? NotFound() : View(item);
        }

        // ================= DELETE =================
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _service.GetByIdAsync(id);
            return item == null ? NotFound() : View(item);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // ================= AJAX =================
        [HttpGet]
        public async Task<IActionResult> GetByCountry(int countryId)
        {
            var states = await _service.GetActiveStatesByCountryAsync(countryId);

            return Json(states.Select(x => new
            {
                stateId = x.StateId,
                stateName = x.StateName
            }));
        }

        // ================= HELPERS =================
        private async Task LoadCountries(int? selectedId = null)
        {
            ViewBag.CountryId = new SelectList(
                await _service.GetActiveCountriesAsync(),
                "CountryId",
                "CountryName",
                selectedId
            );
        }
    }
}
