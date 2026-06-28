using HMS.Data;
using HMS.Entities;
using HMS.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HMS.Services.Implementations
{
    public class MASCityService : IMASCityService
    {
        private readonly HmsDbContext _context;

        public MASCityService(HmsDbContext context)
        {
            _context = context;
        }

        public Task<List<MASCity>> GetAllAsync()
    => _context.MASCities
        .Include(x => x.Mandal)
            .ThenInclude(m => m.State)
                .ThenInclude(s => s.Country)
        .OrderBy(x => x.CityName)
        .ToListAsync();

        public async Task<MASCity?> GetByIdAsync(int id)
        {
            return await _context.MASCities
                .Include(x => x.Mandal)
                    .ThenInclude(m => m.State)
                        .ThenInclude(s => s.Country)
                .FirstOrDefaultAsync(x => x.CityId == id);
        }

        public async Task<bool> CreateAsync(MASCity city)
        {
            _context.MASCities.Add(city);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(MASCity city)
        {
            _context.MASCities.Update(city);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var city = await _context.MASCities.FindAsync(id);
            if (city == null) return false;
            _context.MASCities.Remove(city);
            await _context.SaveChangesAsync();
            return true;
        }

       
    }
}
