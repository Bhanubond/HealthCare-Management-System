using HMS.Models;

namespace HMS.Services.Interfaces
{
    public interface ITreatmentService
    {
        Task<List<TreatmentQueueVm>> GetTreatmentQueue(int deptId, DateTime fromDate, DateTime toDate);
        Task<object> GetTreatmentScreenAsync(int deptId, int patientId);

        //Task<TreatmentQueueVm?> GetPatientById(int patientId);
    }
}
