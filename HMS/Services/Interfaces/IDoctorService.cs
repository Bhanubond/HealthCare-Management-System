using HMS.Models;
using HMS.Entities;

namespace HMS.Services.Interfaces
{
    public interface IDoctorService
    {
        Task<List<Doctor>> GetAllAsync();
        Task<Doctor?> GetByIdAsync(long id);
        Task<Doctor?> GetForDeleteAsync(long id);
        Task<DoctorFormViewModel> BuildCreateAsync();
        Task<DoctorFormViewModel?> BuildEditAsync(long id);
        Task<DoctorFormViewModel> BuildFormAsync(Doctor doctor);
        Task<bool> CreateAsync(Doctor doctor);
        Task<bool> UpdateAsync(Doctor doctor);
        Task<bool> DeleteAsync(long id);
        Task<List<MASState>> GetStatesByCountryAsync(int countryId);
        Task<List<MASMandal>> GetMandalsByStateAsync(int stateId);
        Task<List<MASCity>> GetCitiesByMandalAsync(int mandalId);
    }
}
