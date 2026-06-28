using HMS.Entities;
using HMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace HMS.Controllers
{
    public class MASCityController : BaseController
    {
        private readonly IMASCountryService _countryService;
        private readonly IMASStateService _stateService;
        private readonly IMASMandalService _mandalService;
        private readonly IMASCityService _cityService;

        public MASCityController(
            IMASCityService cityService,
            IMASCountryService countryService,
            IMASStateService stateService,
            IMASMandalService mandalService)
        {
            _cityService = cityService;
            _countryService = countryService;
            _stateService = stateService;
            _mandalService = mandalService;
        }

        private async Task PrepareLookupsAsync(int? selectedStateId = null, int? selectedMandalId = null)
        {
            if (selectedStateId.HasValue)
            {
                SetLookup(nameof(MASCity.MandalId),
                    BuildSelectList(
                        await _mandalService.GetActiveMandalsByStateAsync(selectedStateId.Value),
                        nameof(MASMandal.MandalId),
                        nameof(MASMandal.MandalName),
                        selectedMandalId
                    ));
            }
            else
            {
                SetLookup(nameof(MASCity.MandalId),
                    new List<SelectListItem>());
            }
        }

        public async Task<IActionResult> Index(int? countryId, int? stateId, int? mandalId)
        {
            ViewBag.CountryId = new SelectList(await _countryService.GetActiveCountriesAsync(),"CountryId","CountryName",countryId);

            var states = countryId.HasValue
                ? await _stateService.GetActiveStatesByCountryAsync(countryId.Value)
                : new List<MASState>();

            ViewBag.StateId = new SelectList(
                states,
                "StateId",
                "StateName",
                stateId
            );

            var mandals = stateId.HasValue
                ? await _mandalService.GetActiveMandalsByStateAsync(stateId.Value)
                : new List<MASMandal>();

            ViewBag.MandalId = new SelectList(
                mandals,
                "MandalId",
                "MandalName",
                mandalId
            );

            var data = await _cityService.GetAllAsync();

            if (countryId.HasValue)
                data = data.Where(x => x.Mandal.State.CountryId == countryId.Value).ToList();

            if (stateId.HasValue)
                data = data.Where(x => x.Mandal.StateId == stateId.Value).ToList();

            if (mandalId.HasValue)
                data = data.Where(x => x.MandalId == mandalId.Value).ToList();

            return View(data);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.CountryId = new SelectList(await _countryService.GetActiveCountriesAsync(), "CountryId", "CountryName");
            ViewBag.StateId = new SelectList(new List<MASState>(), "StateId", "StateName");
            ViewBag.MandalId = new SelectList(new List<MASMandal>(), "MandalId", "MandalName");

            return View(new MASCity
            {
                IsActive = true,
                IsDeleted = false
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MASCity model)
        {
            await PrepareLookupsAsync(model.MandalId);

            if (!ModelState.IsValid)
            {
                Error("Please correct the highlighted fields and try again.");
                return View(model);
            }

            await _cityService.CreateAsync(model);
            Success("City created successfully.");
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _cityService.GetByIdAsync(id);

            if (item == null)
                return NotFound();

            var countryId = item.Mandal?.State?.CountryId;
            var stateId = item.Mandal?.StateId;

            // COUNTRIES
            ViewBag.CountryId = new SelectList(
                await _countryService.GetActiveCountriesAsync(),
                "CountryId",
                "CountryName",
                countryId
            );

            // STATES
            ViewBag.StateId = new SelectList(
                countryId.HasValue
                    ? await _stateService.GetActiveStatesByCountryAsync(countryId.Value)
                    : new List<MASState>(),
                "StateId",
                "StateName",
                stateId
            );

            // MANDALS
            ViewBag.MandalId = new SelectList(
                stateId.HasValue
                    ? await _mandalService.GetActiveMandalsByStateAsync(stateId.Value)
                    : new List<MASMandal>(),
                "MandalId",
                "MandalName",
                item.MandalId
            );

            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MASCity model)
        {
            await PrepareLookupsAsync(model.MandalId);

            if (!ModelState.IsValid)
            {
                Error("Please correct the highlighted fields and try again.");
                return View(model);
            }

            await _cityService.UpdateAsync(model);
            Success("City updated successfully.");
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var item = await _cityService.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _cityService.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(MASCity model)
        {
            if (await _cityService.DeleteAsync(model.CityId))
            {
                Success("City deleted successfully.");
            }
            else
            {
                Error("City record was not found.");
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

        [HttpGet]
        public async Task<IActionResult> GetMandalsByState(int stateId)
        {
            var mandals = await _mandalService.GetActiveMandalsByStateAsync(stateId);

            return Json(mandals.Select(x => new
            {
                mandalId = x.MandalId,
                mandalName = x.MandalName
            }));
        }
    }
}
