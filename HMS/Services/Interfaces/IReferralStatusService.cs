using HMS.Entities;
using HMS.Models;

namespace HMS.Services.Interfaces
{
    public interface IReferralStatusService
    {
        Task CreateReferralsAsync(int patientId, int fromDeptId, List<int> toDeptIds, Dictionary<int, string> reasons);

        Task<List<ReferralStatus>> GetByPatientIdAsync(int patientId);

        Task UpdateStatusAsync(int referredId, string status);

        Task<List<ReferralStatus>> GetByDepartmentAsync(int deptId);

        Task<List<ReferralDisplayVm>> GetReferralsByPatientAsync(int patientId);

        Task CompleteReferralAsync(int referredId);
    }
}
