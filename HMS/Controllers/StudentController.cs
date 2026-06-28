using HMS.Entities;
using HMS.Models;
using HMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HMS.Controllers
{
    public class StudentController : BaseController
    {
        private readonly IStudentService _service;

        public StudentController(IStudentService service)
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
        public async Task<IActionResult> Create(StudentFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                Error("Please correct the highlighted fields and try again.");
                vm = await _service.BuildFormAsync(vm.Student);
                return View(vm);
            }

            await _service.CreateAsync(vm.Student);
            Success("Student created successfully.");
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var vm = await _service.BuildEditAsync(id);
            if (vm == null)
            {
                return NotFound();
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(StudentFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                Error("Please correct the highlighted fields and try again.");
                vm = await _service.BuildFormAsync(vm.Student);
                return View(vm);
            }

            await _service.UpdateAsync(vm.Student);
            Success("Student updated successfully.");
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(long id)
        {
            var student = await _service.GetDetailsAsync(id);

            if (student == null)
                return NotFound();

            return View(student);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(long id)
        {
            var student = await _service.GetForDeleteAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Student student)
        {
            if (await _service.DeleteAsync(student.StudentId))
            {
                Success("Student deleted successfully.");
            }
            else
            {
                Error("Student record was not found.");
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<JsonResult> GetCourseYears(int courseId)
        {
            return Json((await _service.GetCourseYearsByCourseAsync(courseId)).Select(x => new { x.CourseYearId, x.YearName }).ToList());
        }

        [HttpGet]
        public async Task<JsonResult> GetStatesByCountry(int countryId)
        {
            return Json((await _service.GetStatesByCountryAsync(countryId)).Select(x => new { x.StateId, x.StateName }).ToList());
        }

        [HttpGet]
        public async Task<JsonResult> GetMandalsByState(int stateId)
        {
            return Json((await _service.GetMandalsByStateAsync(stateId)).Select(x => new { x.MandalId, x.MandalName }).ToList());
        }

        [HttpGet]
        public async Task<JsonResult> GetCitiesByMandal(int mandalId)
        {
            return Json((await _service.GetCitiesByMandalAsync(mandalId)).Select(x => new { x.CityId, x.CityName }).ToList());
        }
    }
}
