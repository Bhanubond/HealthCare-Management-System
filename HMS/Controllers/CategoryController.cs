using HMS.Data;
using HMS.Entities;
using HMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HMS.Controllers
{
    public class CategoryController : BaseController
    {
        private readonly ICategoryService _service;

        public CategoryController(ICategoryService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _service.GetAllAsync();
            return View(data);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(MASCategory model)
        {
            await _service.CreateAsync(model);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var data = await _service.GetByIdAsync(id);
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(MASCategory model)
        {
            await _service.UpdateAsync(model);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var data = await _service.GetByIdAsync(id);

            if (data == null)
            {
                return NotFound();
            }

            return View(data);
        }
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}
