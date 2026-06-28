using HMS.Data;
using HMS.Entities;
using HMS.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HMS.Services.Implementations
{
    public class MASStateService : IMASStateService
    {
        private readonly HmsDbContext _context;

        public MASStateService(HmsDbContext context)
        {
            _context = context;
        }

        public Task<List<MASState>> GetAllAsync()
            => _context.MASStates.Include(x => x.Country).OrderBy(x => x.StateName).ToListAsync();

        public Task<MASState?> GetByIdAsync(int id)
    => _context.MASStates
        .Include(x => x.Country)
        .FirstOrDefaultAsync(x => x.StateId == id);

        public Task<List<MASCountry>> GetActiveCountriesAsync()
            => _context.MASCountries.Where(x => x.IsActive).OrderBy(x => x.CountryName).ToListAsync();

        public async Task<bool> CreateAsync(MASState state)
        {
            _context.MASStates.Add(state);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(MASState state)
        {
            _context.MASStates.Update(state);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var state = await _context.MASStates.FindAsync(id);
            if (state == null) return false;
            _context.MASStates.Remove(state);
            await _context.SaveChangesAsync();
            return true;
        }

        public Task<List<MASState>> GetActiveStatesByCountryAsync(int countryId)
    => _context.MASStates
        .Where(x => x.IsActive && x.CountryId == countryId)
        .OrderBy(x => x.StateName)
        .ToListAsync();
    }
}
