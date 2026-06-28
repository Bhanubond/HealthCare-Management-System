using HMS.Entities;

namespace HMS.Services.Interfaces
{
    public interface IMASCountryService
    {
        Task<List<MASCountry>> GetAllAsync();
        Task<MASCountry?> GetByIdAsync(int id);
        Task<bool> CreateAsync(MASCountry country);
        Task<bool> UpdateAsync(MASCountry country);
        Task<bool> DeleteAsync(int id);

        Task<List<MASCountry>> GetActiveCountriesAsync();
    }
}
