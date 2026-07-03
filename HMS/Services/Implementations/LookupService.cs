using HMS.Data;
using HMS.Entities;
using HMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HMS.Services.Implementations
{
    public class LookupService : ILookupService
    {
        private readonly HmsDbContext _db;

        public LookupService(HmsDbContext db)
        {
            _db = db;
        }

        public async Task<List<SelectListItem>> GetDoctorsAsync()
        {
            return await _db.Doctors
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.DoctorName)
                .Select(x => new SelectListItem
                {
                    Value = x.DoctorId.ToString(),
                    Text = x.DoctorName
                })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetStudentsAsync()
        {
            return await _db.Students
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.StudentName)
                .Select(x => new SelectListItem
                {
                    Value = x.StudentId.ToString(),
                    Text = x.StudentName
                })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetDepartmentsAsync()
        {
            return await _db.MASDepartments
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.DeptName)
                .Select(x => new SelectListItem
                {
                    Value = x.DeptId.ToString(),
                    Text = x.DeptName
                })
                .ToListAsync();
        }

        public async Task<List<Doctor>> GetDoctorsByDepartmentAsync(int departmentId)
        {
            return await _db.Doctors
                .AsNoTracking()
                .Where(x => x.IsActive && x.DepartmentId == departmentId)
                .OrderBy(x => x.DoctorName)
                .ToListAsync();
        }

        public async Task<List<Student>> GetStudentsByDepartmentAsync(int departmentId)
        {
            return await _db.Students
                .AsNoTracking()
                .Where(x => x.IsActive && x.DepartmentId == departmentId)
                .OrderBy(x => x.StudentName)
                .ToListAsync();
        }
    }
}