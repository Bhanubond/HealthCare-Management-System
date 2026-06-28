using HMS.Data;
using HMS.Entities;
using HMS.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HMS.Services.Implementations
{
    public class MASCountryService : IMASCountryService
    {
        private readonly HmsDbContext _context;

        public MASCountryService(HmsDbContext context)
        {
            _context = context;
        }

        public Task<List<MASCountry>> GetAllAsync()
            => _context.MASCountries.OrderBy(x => x.CountryName).ToListAsync();

        public Task<MASCountry?> GetByIdAsync(int id)
            => _context.MASCountries.FindAsync(id).AsTask();

        public async Task<bool> CreateAsync(MASCountry country)
        {
            _context.MASCountries.Add(country);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(MASCountry country)
        {
            _context.MASCountries.Update(country);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var country = await _context.MASCountries.FindAsync(id);
            if (country == null) return false;
            _context.MASCountries.Remove(country);
            await _context.SaveChangesAsync();
            return true;
        }

        public Task<List<MASCountry>> GetActiveCountriesAsync()
     => _context.MASCountries
         .Where(x => x.IsActive)
         .OrderBy(x => x.CountryName)
         .ToListAsync();
    }
}
