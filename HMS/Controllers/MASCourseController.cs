using HMS.Entities;
using HMS.Models;
using HMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace HMS.Controllers
{
    public class MASCourseController : BaseController
    {
        private readonly IMASCourseService _service;

        public MASCourseController(IMASCourseService service)
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
            return View(new MASCourse
            {
                IsActive = true
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MASCourse model)
        {

            if (!ModelState.IsValid)
            {
                Error("Please correct the highlighted fields and try again.");
                return View(model);
            }

            if (!model.IsActive)
            {
                model.IsActive = true;
            }

            await _service.CreateAsync(model);
            Success("Course created successfully.");
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
        public async Task<IActionResult> Edit(MASCourse model)
        {
            if (!ModelState.IsValid)
            {
                Error("Please correct the highlighted fields and try again.");
                return View(model);
            }

            await _service.UpdateAsync(model);
            Success("Course updated successfully.");
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
        public async Task<IActionResult> DeleteConfirmed(MASCourse model)
        {
            if (await _service.DeleteAsync(model.CourseId))
            {
                Success("Course deleted successfully.");
            }
            else
            {
                Error("Course record was not found.");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
