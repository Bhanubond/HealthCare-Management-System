using HMS.Entities;

namespace HMS.Services.Interfaces
{
    public interface IMASStateService
    {
        Task<List<MASState>> GetAllAsync();
        Task<MASState?> GetByIdAsync(int id);
        Task<List<MASCountry>> GetActiveCountriesAsync();
        Task<bool> CreateAsync(MASState state);
        Task<bool> UpdateAsync(MASState state);
        Task<bool> DeleteAsync(int id);
        Task<List<MASState>> GetActiveStatesByCountryAsync(int countryId);
    }
}
