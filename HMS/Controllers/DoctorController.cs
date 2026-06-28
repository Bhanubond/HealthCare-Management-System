using HMS.Entities;
using HMS.Models;
using HMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HMS.Controllers
{
    public class DoctorController : BaseController
    {
        private readonly IDoctorService _service;

        public DoctorController(IDoctorService service)
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
        public async Task<IActionResult> Create(DoctorFormViewModel vm)
        {

            if (!ModelState.IsValid)
            {
                Error("Please correct the highlighted fields and try again.");
                vm = await _service.BuildFormAsync(vm.Doctor);
                return View(vm);
            }

            await _service.CreateAsync(vm.Doctor);
            Success("Doctor created successfully.");
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var vm = await _service.BuildEditAsync(id);

            if (vm == null)
                return NotFound();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DoctorFormViewModel vm)
        {

            if (!ModelState.IsValid)
            {
                Error("Please correct the highlighted fields and try again.");
                vm = await _service.BuildFormAsync(vm.Doctor);
                return View(vm);
            }

            await _service.UpdateAsync(vm.Doctor);
            Success("Doctor updated successfully.");
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(long id)
        {
            var item = await _service.GetByIdAsync(id);

            if (item == null)
                return NotFound();
            return View(item);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(long id)
        {
            var item = await _service.GetForDeleteAsync(id);

            if (item == null)
                return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Doctor model)
        {
            if (await _service.DeleteAsync(model.DoctorId))
                Success("Doctor deleted successfully.");
            else
                Error("Doctor record was not found.");

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<JsonResult> GetStatesByCountry(int countryId)
        {
            var data = await _service.GetStatesByCountryAsync(countryId);

            return Json(data.Select(x => new
            {
                stateId = x.StateId,
                stateName = x.StateName
            }).ToList());
        }

        [HttpGet]
        public async Task<JsonResult> GetMandalsByState(int stateId)
        {
            var data = await _service.GetMandalsByStateAsync(stateId);

            return Json(data.Select(x => new
            {
                mandalId = x.MandalId,
                mandalName = x.MandalName
            }).ToList());
        }

        [HttpGet]
        public async Task<JsonResult> GetCitiesByMandal(int mandalId)
        {
            var data = await _service.GetCitiesByMandalAsync(mandalId);

            return Json(data.Select(x => new
            {
                cityId = x.CityId,
                cityName = x.CityName
            }).ToList());
        }
    }
}