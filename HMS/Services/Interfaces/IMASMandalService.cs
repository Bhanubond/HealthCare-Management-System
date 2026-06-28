using HMS.Entities;

namespace HMS.Services.Interfaces
{
    public interface IMASMandalService
    {
        Task<List<MASMandal>> GetAllAsync();
        Task<MASMandal?> GetByIdAsync(int id);

        Task<List<MASMandal>> GetActiveMandalsByStateAsync(int stateId);

        Task<bool> CreateAsync(MASMandal mandal);
        Task<bool> UpdateAsync(MASMandal mandal);
        Task<bool> DeleteAsync(int id);
    }
}