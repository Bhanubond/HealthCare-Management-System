using HMS.Data;
using HMS.Entities;
using HMS.Models;
using HMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
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

        public async Task<GMCasesheetViewVm> GetCaseSheetPatient(int patientId)
        {
            var param = new SqlParameter("@PatientId", patientId);

            var data = await _db.Set<GMCasesheetViewVm>()
                .FromSqlRaw("EXEC SP_GetPatientDetailsById @PatientId", param)
                .AsNoTracking()
                .ToListAsync();

            var model = data.FirstOrDefault() ?? new GMCasesheetViewVm();
            model.PatientId = patientId;
            return model;
        }

        public async Task<TreatmentContextVm> GetLatestTreatmentContextAsync(int DeptId, int patientId)
        {
            var allotment = await _db.StudentAllotments
                .AsNoTracking()
                .Where(x => x.PatientId == patientId && x.DeptId == DeptId)
                .OrderByDescending(x => x.AllotId)
                .FirstOrDefaultAsync();

            var referral = await _db.ReferralStatuses
                .AsNoTracking()
                .Where(x => x.PatientId == patientId && x.ReferredId == allotment.ReferredId)
                .OrderByDescending(x => x.ReferredId)
                .FirstOrDefaultAsync();

            var doctorName = string.Empty;
            if (allotment?.DoctorId.HasValue == true)
            {
                doctorName = await _db.Doctors
                    .AsNoTracking()
                    .Where(x => x.DoctorId == allotment.DoctorId.Value)
                    .Select(x => x.DoctorName)
                    .FirstOrDefaultAsync() ?? string.Empty;
            }

            var studentName = string.Empty;
            if (allotment?.StudentId.HasValue == true)
            {
                studentName = await _db.Students
                    .AsNoTracking()
                    .Where(x => x.StudentId == allotment.StudentId.Value)
                    .Select(x => x.StudentName)
                    .FirstOrDefaultAsync() ?? string.Empty;
            }

            return new TreatmentContextVm
            {
                AllotId = allotment?.AllotId,
                ReferredId = referral?.ReferredId,
                DeptId = allotment?.DeptId,
                DoctorId = allotment?.DoctorId ?? 0,
                DoctorName = doctorName,
                StudentId = allotment?.StudentId ?? 0,
                StudentName = studentName
            };
        }
    }
}