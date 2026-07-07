using HMS.Entities;
using HMS.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HMS.Services.Interfaces
{
    public interface ILookupService
    {
        Task<List<SelectListItem>> GetDoctorsAsync();

        Task<List<SelectListItem>> GetStudentsAsync();

        Task<List<SelectListItem>> GetDepartmentsAsync();
        Task<List<Doctor>> GetDoctorsByDepartmentAsync(int departmentId);
        Task<List<Student>> GetStudentsByDepartmentAsync(int departmentId);
        Task<TreatmentContextVm> GetLatestTreatmentContextAsync(int DeptId, int patientId);
        Task<GMCasesheetViewVm> GetCaseSheetPatient(int patientId);
    }
}