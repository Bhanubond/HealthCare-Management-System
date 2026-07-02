using HMS.Entities;
using HMS.Models;

namespace HMS.Services.Interfaces
{
    public interface IGeneralMedicineServices
    {
        Task<List<TreatmentPatientVm>> GetPendingTreatmentPatients();
        Task<GMCasesheetViewVm> GetCaseSheetPatient(int patientId);
        Task<List<MASMedication>> GetActiveMedications();
        Task<GMCasesheetScreenVm> GetTreatmentScreenAsync(int patientId);
        Task SaveCaseSheet(GMCasesheetSaveVm model);
        //Task<List<GMCasesheetSearchVm>> GetCompletedCases();

        Task<List<GMCasesheetSearchVm>> GetCompletedCases(DateTime from, DateTime to);
        Task<GMCasesheetScreenVm> GetCaseSheetById(int gmId);
        Task UpdateCaseSheet(GMCasesheetSaveVm model);
    }
}
