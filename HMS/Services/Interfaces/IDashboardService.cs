using HMS.Models;

namespace HMS.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<List<DashboardDepartmentViewModel>> GetDepartmentsAsync();

        Task<List<DashboardScreenCardViewModel>> GetScreensAsync(int departmentId);
    }
}