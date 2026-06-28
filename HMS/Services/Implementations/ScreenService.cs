using HMS.Data;
using HMS.Entities;
using HMS.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HMS.Services.Implementations
{
    public class ScreenService : IScreenService
    {
        private readonly HmsDbContext _context;

        public ScreenService(HmsDbContext context)
        {
            _context = context;
        }

        public async Task<List<MASScreen>> GetAllAsync()
        {
            return await _context.MASScreens.OrderBy(x => x.OrderDisplay ?? int.MaxValue).ThenBy(x => x.ScreenDisplayName).ToListAsync();
        }

        public async Task<MASScreen?> GetByIdAsync(int id)
        {
            return await _context.MASScreens.FindAsync(id);
        }

        public async Task<bool> CreateAsync(MASScreen screen)
        {
            _context.MASScreens.Add(screen);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(MASScreen screen)
        {
            _context.MASScreens.Update(screen);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var screen = await _context.MASScreens.FindAsync(id);
            if (screen == null) return false;
            _context.MASScreens.Remove(screen);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
