using HMS.Data;
using HMS.Entities;
using HMS.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HMS.Services.Implementations
{
    public class MASDeptScreenMappingService : IMASDeptScreenMappingService
    {
        private readonly HmsDbContext _context;

        public MASDeptScreenMappingService(HmsDbContext context)
        {
            _context = context;
        }

        public Task<List<MASDeptScreenMapping>> GetAllAsync()
            => _context.MASDeptScreenMappings.Include(x => x.Department).Include(x => x.Screen).OrderByDescending(x => x.DeptScreenId).ToListAsync();

        public Task<MASDeptScreenMapping?> GetByIdAsync(int id)
            => _context.MASDeptScreenMappings.FindAsync(id).AsTask();

        public Task<List<MASDepartment>> GetActiveDepartmentsAsync()
            => _context.MASDepartments.Where(x => x.IsActive).OrderBy(x => x.DeptName).ToListAsync();

        public Task<List<MASScreen>> GetActiveScreensAsync()
            => _context.MASScreens.Where(x => x.IsActive).OrderBy(x => x.ScreenDisplayName).ToListAsync();

        public async Task<bool> CreateAsync(MASDeptScreenMapping mapping)
        {
            _context.MASDeptScreenMappings.Add(mapping);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(MASDeptScreenMapping mapping)
        {
            _context.MASDeptScreenMappings.Update(mapping);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var mapping = await _context.MASDeptScreenMappings.FindAsync(id);
            if (mapping == null) return false;
            _context.MASDeptScreenMappings.Remove(mapping);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
