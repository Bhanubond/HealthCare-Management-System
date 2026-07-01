using HMS.Models;

namespace HMS.Services.Interfaces
{
    public interface IAllotmentService
    {
        //Task<List<AllotmentViewModel>> GetPatientsByDepartment(int deptId);
        Task<List<AllotmentViewModel>> GetPatientsByDepartment(int deptId, DateTime fromDate, DateTime toDate);

        Task SaveAllotment(StudentAllotmentViewModel model);

        Task<StudentAllotmentViewModel> GetAllotFormData(int patientId, int deptId);
    }
}
