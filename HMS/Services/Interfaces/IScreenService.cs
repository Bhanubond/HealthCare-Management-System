using HMS.Entities;

namespace HMS.Services.Interfaces
{
    public interface IScreenService
    {
        Task<List<MASScreen>> GetAllAsync();
        Task<MASScreen?> GetByIdAsync(int id);
        Task<bool> CreateAsync(MASScreen screen);
        Task<bool> UpdateAsync(MASScreen screen);
        Task<bool> DeleteAsync(int id);
    }
}
