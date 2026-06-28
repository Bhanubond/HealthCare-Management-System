using HMS.Data;
using HMS.Entities;
using HMS.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HMS.Services.Implementations
{
    public class MASDepartmentService : IMASDepartmentService
    {
        private readonly HmsDbContext _context;

        public MASDepartmentService(HmsDbContext context)
        {
            _context = context;
        }

        public async Task<List<MASDepartment>> GetAllAsync()
        {
            return await _context.MASDepartments.OrderBy(x => x.DisplayOrder ?? int.MaxValue).ThenBy(x => x.DeptName).ToListAsync();
        }

        public async Task<MASDepartment?> GetByIdAsync(int id)
        {
            return await _context.MASDepartments.FindAsync(id);
        }

        public async Task<bool> CreateAsync(MASDepartment department)
        {
            _context.MASDepartments.Add(department);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(MASDepartment department)
        {
            _context.MASDepartments.Update(department);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var department = await _context.MASDepartments.FindAsync(id);
            if (department == null) return false;
            _context.MASDepartments.Remove(department);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
