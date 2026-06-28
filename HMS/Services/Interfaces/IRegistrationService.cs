using HMS.Entities;
using HMS.Models;

namespace HMS.Services.Interfaces
{
    public interface IRegistrationService
    {
        Task<List<OPDPatientRegistration>> GetAllAsync();
        Task<OPDPatientRegistration?> GetByIdAsync(long id);
        Task<OPDPatientRegistration?> GetForDeleteAsync(long id);

        Task<RegistrationFormViewModel> BuildCreateAsync();
        Task<RegistrationFormViewModel?> BuildEditAsync(long id);
        Task<RegistrationFormViewModel> BuildFormAsync(OPDPatientRegistration patient);

        Task<long> CreateAsync(OPDPatientRegistration patient);
        Task<bool> UpdateAsync(OPDPatientRegistration patient);
        Task<bool> DeleteAsync(long id);

        Task<List<MASState>> GetStatesByCountryAsync(int countryId);
        Task<List<MASMandal>> GetMandalsByStateAsync(int stateId);
        Task<List<MASCity>> GetCitiesByMandalAsync(int mandalId);

        Task<bool> IsDuplicateAsync(string phone, string aadharNo);

        Task<OPDSearchViewModel> SearchOPDAsync(OPDSearchViewModel model);
        Task<string> GenerateUHIDAsync();
        Task<string> GenerateOpNoAsync();
        Task<MASHospital?> GetHospitalAsync();
    }
}