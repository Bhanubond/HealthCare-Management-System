using HMS.Data;
using HMS.Entities;
using HMS.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace HMS.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly HmsDbContext _context;

        public CategoryService(HmsDbContext context)
        {
            _context = context;
        }

        public async Task<List<MASCategory>> GetAllAsync()
        {
            return await _context.MASCategory
                .Where(x => x.DelInd == false)
                .ToListAsync();
        }

        public async Task<MASCategory> GetByIdAsync(int id)
        {
            return await _context.MASCategory
                .FirstOrDefaultAsync(x => x.CategoryId == id);
        }

        public async Task CreateAsync(MASCategory model)
        {
            model.CreatedDate = DateTime.Now;

            _context.MASCategory.Add(model);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(MASCategory model)
        {
            var data = await _context.MASCategory.FindAsync(model.CategoryId);

            data.CategoryName = model.CategoryName;
            data.DiscountPer = model.DiscountPer;
            data.IsActive = model.IsActive;
            data.UpdatedDate = DateTime.Now;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var data = await _context.MASCategory.FindAsync(id);

            data.DelInd = true;
            data.UpdatedDate = DateTime.Now;

            await _context.SaveChangesAsync();
        }
    }
}
