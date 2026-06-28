using HMS.Data;
using HMS.Entities;
using HMS.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HMS.Services.Implementations
{
    public class MASDesignationService : IMASDesignationService
    {
        private readonly HmsDbContext _context;

        public MASDesignationService(HmsDbContext context)
        {
            _context = context;
        }

        public Task<List<MASDesignation>> GetAllAsync()
            => _context.MASDesignations.OrderBy(x => x.DesignationId).ToListAsync();

        public Task<MASDesignation?> GetByIdAsync(int id)
            => _context.MASDesignations.FindAsync(id).AsTask();

        public async Task<bool> CreateAsync(MASDesignation designation)
        {
            _context.MASDesignations.Add(designation);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(MASDesignation designation)
        {
            _context.MASDesignations.Update(designation);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var designation = await _context.MASDesignations.FindAsync(id);
            if (designation == null) return false;
            _context.MASDesignations.Remove(designation);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
