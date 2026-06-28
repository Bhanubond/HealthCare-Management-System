using HMS.Entities;

namespace HMS.Services.Interfaces
{
    public interface IMASCityService
    {
        Task<List<MASCity>> GetAllAsync();
        Task<MASCity?> GetByIdAsync(int id);
        Task<bool> CreateAsync(MASCity city);
        Task<bool> UpdateAsync(MASCity city);
        Task<bool> DeleteAsync(int id);

        
    }
}
