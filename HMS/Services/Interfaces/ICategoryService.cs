using HMS.Entities;
using System.Collections.Generic;

namespace HMS.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<List<MASCategory>> GetAllAsync();
        Task<MASCategory> GetByIdAsync(int id);
        Task CreateAsync(MASCategory model);
        Task UpdateAsync(MASCategory model);
        Task DeleteAsync(int id);
    }
}
