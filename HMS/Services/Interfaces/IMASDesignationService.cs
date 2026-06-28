using HMS.Entities;

namespace HMS.Services.Interfaces
{
    public interface IMASDesignationService
    {
        Task<List<MASDesignation>> GetAllAsync();
        Task<MASDesignation?> GetByIdAsync(int id);
        Task<bool> CreateAsync(MASDesignation designation);
        Task<bool> UpdateAsync(MASDesignation designation);
        Task<bool> DeleteAsync(int id);
    }
}
