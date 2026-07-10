using HMS.Entities;
using HMS.Models;

namespace HMS.Services.Interfaces
{
    public interface ITreatmentlistServices
    {
        Task<List<TreatmentServiceListVM>> GetAllAsync();

        Task<TreatmentServices?> GetByIdAsync(int id);

        Task AddAsync(TreatmentServices model);

        Task UpdateAsync(TreatmentServices model);

        Task DeleteAsync(int id);

        Task<List<MASDepartment>> GetDepartmentsAsync();
    }
}