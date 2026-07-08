using HMS.Entities;
using HMS.Models;

namespace HMS.Services.Interfaces
{
    public interface IEmergencyService
    {
        Task<EMRCasesheetScreenVm> GetTreatmentScreenAsync(int DeptId, int patientId);
        Task SaveCaseSheet(EMRCasesheetScreenVm model);
        Task<List<EMRCompletedCaseVm>> GetCompletedCases(DateTime from, DateTime to);
        Task<EMRCasesheetScreenVm> GetCaseSheetById(int EMRId);
        Task UpdateCaseSheet(EMRCasesheetScreenVm model);
        Task<List<EMRApprovalQueueVm>> GetApprovalQueue(DateTime fromDate, DateTime toDate);
        Task<string> ProcessApprovalFlow(int EMRId);

    }
}
