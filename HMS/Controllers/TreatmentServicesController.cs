using HMS.Entities;
using HMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HMS.Controllers
{
    public class TreatmentlistServicesController : Controller
    {
        private readonly ITreatmentlistServices _service;

        public TreatmentlistServicesController(ITreatmentlistServices service)
        {
            _service = service;
        }

        private async Task LoadDepartments()
        {
            var departments = await _service.GetDepartmentsAsync();

            ViewBag.Departments = new SelectList(
                departments,
                "DeptId",
                "DeptName"
            );
        }

        public async Task<IActionResult> Index()
        {
            var data = await _service.GetAllAsync();
            return View(data);
        }


        public async Task<IActionResult> Details(int id)
        {
            var data = await _service.GetByIdAsync(id);

            if (data == null)
                return NotFound();

            return View(data);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadDepartments();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TreatmentServices model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDepartments();
                return View(model);
            }

            await _service.AddAsync(model);

            TempData["Success"] = "Treatment Service Created Successfully.";

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var data = await _service.GetByIdAsync(id);

            if (data == null)
                return NotFound();

            await LoadDepartments();

            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TreatmentServices model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDepartments();
                return View(model);
            }

            await _service.UpdateAsync(model);

            TempData["Success"] = "Treatment Service Updated Successfully.";

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var data = await _service.GetByIdAsync(id);

            if (data == null)
                return NotFound();

            return View(data);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteAsync(id);

            TempData["Success"] = "Treatment Service Deleted Successfully.";

            return RedirectToAction(nameof(Index));
        }
    }

}
