using HMS.Entities;
using HMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace HMS.Controllers
{
    public class MASMandalController : BaseController
    {
        private readonly IMASMandalService _service;
        private readonly IMASStateService _stateService;

        public MASMandalController(IMASMandalService service, IMASStateService stateService)
        {
            _service = service;
            _stateService = stateService;
        }

        private async Task PrepareLookupsAsync(int? selectedCountryId = null, int? selectedStateId = null)
        {
            var states = selectedCountryId.HasValue
                ? await _stateService.GetActiveStatesByCountryAsync(selectedCountryId.Value)
                : new List<MASState>();

            SetLookup(nameof(MASMandal.StateId),
                BuildSelectList(
                    states,
                    nameof(MASState.StateId),
                    nameof(MASState.StateName),
                    selectedStateId
                ));
        }

        public async Task<IActionResult> Index(int? countryId, int? stateId)
        {
            // COUNTRIES
            ViewBag.CountryId = new SelectList(
                await _stateService.GetActiveCountriesAsync(),
                "CountryId",
                "CountryName",
                countryId
            );

            // STATES (FILTERED)
            ViewBag.StateId = new SelectList(
                countryId.HasValue
                    ? await _stateService.GetActiveStatesByCountryAsync(countryId.Value)
                    : new List<MASState>(),
                "StateId",
                "StateName",
                stateId
            );

            // DATA
            var data = await _service.GetAllAsync();

            if (countryId.HasValue)
                data = data.Where(x => x.State.CountryId == countryId.Value).ToList();

            if (stateId.HasValue)
                data = data.Where(x => x.StateId == stateId.Value).ToList();

            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.CountryId = new SelectList(
                await _stateService.GetActiveCountriesAsync(),
                "CountryId",
                "CountryName"
            );

            ViewBag.StateId = new SelectList(new List<MASState>(), "StateId", "StateName");

            return View(new MASMandal
            {
                IsActive = true,
                IsDeleted = false
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MASMandal model)
        {
            await PrepareLookupsAsync(model.StateId);

            if (!ModelState.IsValid)
            {
                Error("Please correct the highlighted fields and try again.");
                return View(model);
            }

            await _service.CreateAsync(model);
            Success("Mandal created successfully.");
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _service.GetByIdAsync(id);

            if (item == null)
                return NotFound();

            var countryId = item.State?.CountryId;
            var stateId = item.StateId;

            ViewBag.CountryId = new SelectList(
                await _stateService.GetActiveCountriesAsync(),
                "CountryId",
                "CountryName",
                countryId
            );

            ViewBag.StateId = new SelectList(
                countryId.HasValue
                    ? await _stateService.GetActiveStatesByCountryAsync(countryId.Value)
                    : new List<MASState>(),
                "StateId",
                "StateName",
                stateId
            );

            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MASMandal model)
        {
            await PrepareLookupsAsync(model.StateId);

            if (!ModelState.IsValid)
            {
                Error("Please correct the highlighted fields and try again.");
                return View(model);
            }

            await _service.UpdateAsync(model);
            Success("Mandal updated successfully.");
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
        public async Task<IActionResult> DeleteConfirmed(MASMandal model)
        {
            if (await _service.DeleteAsync(model.MandalId))
            {
                Success("Mandal deleted successfully.");
            }
            else
            {
                Error("Mandal record was not found.");
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetStatesByCountry(int countryId)
        {
            var states = await _stateService.GetActiveStatesByCountryAsync(countryId);

            return Json(states.Select(x => new
            {
                stateId = x.StateId,
                stateName = x.StateName
            }));
        }
    }
}
