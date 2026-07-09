using HMS.Models;

namespace HMS.Services.Interfaces
{
    public interface IPediatricsServices
    {
        Task<PedoCasesheetScreenVm> GetTreatmentScreenAsync(int DeptId, int patientId);
        Task SaveCaseSheet(PedoCasesheetScreenVm model);
        Task<List<PedoCompletedCaseVm>> GetCompletedCases(DateTime from, DateTime to);
        Task<PedoCasesheetScreenVm> GetCaseSheetById(int PedoID);
        Task UpdateCaseSheet(PedoCasesheetScreenVm model);
        Task<List<PedoApprovalQueueVm>> GetApprovalQueue(DateTime fromDate, DateTime toDate);
        Task<string> ProcessApprovalFlow(int PedoID);
    }
}
