using HMS.Entities;

namespace HMS.Services.Interfaces
{
    public interface IMASDeptScreenMappingService
    {
        Task<List<MASDeptScreenMapping>> GetAllAsync();
        Task<MASDeptScreenMapping?> GetByIdAsync(int id);
        Task<List<MASDepartment>> GetActiveDepartmentsAsync();
        Task<List<MASScreen>> GetActiveScreensAsync();
        Task<bool> CreateAsync(MASDeptScreenMapping mapping);
        Task<bool> UpdateAsync(MASDeptScreenMapping mapping);
        Task<bool> DeleteAsync(int id);
    }
}
