using HMS.Entities;
using HMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace HMS.Controllers
{
    public class MASCourseYearController : BaseController
    {
        private readonly IMASCourseYearService _service;

        public MASCourseYearController(IMASCourseYearService service)
        {
            _service = service;
        }

        private async Task PrepareLookupsAsync(int? selectedCourseId = null)
        {
            SetLookup(nameof(MASCourseYear.CourseId),
                BuildSelectList(await _service.GetActiveCoursesAsync(),
                    nameof(MASCourse.CourseId),
                    nameof(MASCourse.CourseName),
                    selectedCourseId));
        }

        public async Task<IActionResult> Index(int? courseId)
        {

            ViewBag.Courses = await _service.GetActiveCoursesAsync();
            ViewBag.SelectedCourseId = courseId;

            var data = await _service.GetAllAsync();

            if (courseId.HasValue)
            {
                data = data.Where(x => x.CourseId == courseId.Value).ToList();
            }

            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PrepareLookupsAsync();
            return View(new MASCourseYear { IsActive = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MASCourseYear model)
        {
            await PrepareLookupsAsync(model.CourseId);

            if (!ModelState.IsValid)
            {
                Error("Please correct the highlighted fields and try again.");
                return View(model);
            }

            await _service.CreateAsync(model);
            Success("Course year created successfully.");
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

            await PrepareLookupsAsync(item.CourseId);
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MASCourseYear model)
        {
            await PrepareLookupsAsync(model.CourseId);

            if (!ModelState.IsValid)
            {
                Error("Please correct the highlighted fields and try again.");
                return View(model);
            }

            await _service.UpdateAsync(model);
            Success("Course year updated successfully.");
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
        public async Task<IActionResult> DeleteConfirmed(MASCourseYear model)
        {
            if (await _service.DeleteAsync(model.CourseYearId))
            {
                Success("Course year deleted successfully.");
            }
            else
            {
                Error("Course year record was not found.");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
