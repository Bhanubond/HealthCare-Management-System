using HMS.Entities;

namespace HMS.Services.Interfaces
{
    public interface IMASDepartmentService
    {
        Task<List<MASDepartment>> GetAllAsync();
        Task<MASDepartment?> GetByIdAsync(int id);
        Task<bool> CreateAsync(MASDepartment department);
        Task<bool> UpdateAsync(MASDepartment department);
        Task<bool> DeleteAsync(int id);
    }
}
