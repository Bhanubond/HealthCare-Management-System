using HMS.Entities;
using HMS.Models;

namespace HMS.Services.Interfaces
{
    public interface IGeneralMedicineServices
    {
        Task<List<TreatmentPatientVm>> GetPendingTreatmentPatients();
        Task<GMCasesheetScreenVm> GetTreatmentScreenAsync(int DeptId, int patientId);
        Task SaveCaseSheet(GMCasesheetSaveVm model);
        Task<List<GMCasesheetSearchVm>> GetCompletedCases(DateTime from, DateTime to);
        Task<GMCasesheetScreenVm> GetCaseSheetById(int gmId);
        Task UpdateCaseSheet(GMCasesheetSaveVm model);
        Task<string> ProcessApprovalFlow(int gmId);
        Task<List<GMApprovalQueueVm>> GetApprovalQueue(DateTime fromDate, DateTime toDate);
    }
}
