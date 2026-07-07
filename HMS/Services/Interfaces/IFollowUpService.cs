using HMS.Entities;
using HMS.Models;

namespace HMS.Services.Interfaces
{
    public interface IFollowUpService
    {
        Task<FollowUp?> GetLatestFollowUpAsync(int patientId);

        Task<int?> GetCurrentDepartmentIdAsync(int patientId);

        Task SaveOrUpdateFollowUpAsync(FollowUpSaveVm model);
    }
}