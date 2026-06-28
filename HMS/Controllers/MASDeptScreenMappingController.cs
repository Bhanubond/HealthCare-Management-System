using HMS.Entities;
using HMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace HMS.Controllers
{
    public class MASDeptScreenMappingController : BaseController
    {
        private readonly IMASDeptScreenMappingService _service;

        public MASDeptScreenMappingController(IMASDeptScreenMappingService service)
        {
            _service = service;
        }

        private async Task PrepareLookupsAsync(int? selectedDeptId = null, int? selectedScreenId = null)
        {
            SetLookup(nameof(MASDeptScreenMapping.DeptId),
                BuildSelectList(await _service.GetActiveDepartmentsAsync(),
                    nameof(MASDepartment.DeptId),
                    nameof(MASDepartment.DeptName),
                    selectedDeptId));

            SetLookup(nameof(MASDeptScreenMapping.ScreenId),
                BuildSelectList(await _service.GetActiveScreensAsync(),
                    nameof(MASScreen.ScreenId),
                    nameof(MASScreen.ScreenDisplayName),
                    selectedScreenId));
        }

        public async Task<IActionResult> Index()
        {
            ConfigureList("Department Screen Mappings");
            return View(await _service.GetAllAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PrepareLookupsAsync();
            ConfigureForm("Create Department Screen Mapping", "Create", "Save");
            return View(new MASDeptScreenMapping { IsActive = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MASDeptScreenMapping model)
        {
            ConfigureForm("Create Department Screen Mapping", "Create", "Save");
            await PrepareLookupsAsync(model.DeptId, model.ScreenId);

            if (!ModelState.IsValid)
            {
                Error("Please correct the highlighted fields and try again.");
                return View(model);
            }

            await _service.CreateAsync(model);
            Success("Department screen mapping created successfully.");
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

            await PrepareLookupsAsync(item.DeptId, item.ScreenId);
            ConfigureForm("Edit Department Screen Mapping", "Edit", "Update");
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MASDeptScreenMapping model)
        {
            ConfigureForm("Edit Department Screen Mapping", "Edit", "Update");
            await PrepareLookupsAsync(model.DeptId, model.ScreenId);

            if (!ModelState.IsValid)
            {
                Error("Please correct the highlighted fields and try again.");
                return View(model);
            }

            await _service.UpdateAsync(model);
            Success("Department screen mapping updated successfully.");
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

            ConfigureDetails("Department Screen Mapping Details");
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

            ConfigureDelete("Delete Department Screen Mapping");
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(MASDeptScreenMapping model)
        {
            if (await _service.DeleteAsync(model.DeptScreenId))
            {
                Success("Department screen mapping deleted successfully.");
            }
            else
            {
                Error("Department screen mapping record was not found.");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
