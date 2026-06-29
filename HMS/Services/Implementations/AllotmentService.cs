using HMS.Data;
using HMS.Entities;
using HMS.Models;
using HMS.Services.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace HMS.Services.Implementations
{
    public class AllotmentService : IAllotmentService
    {
        private readonly HmsDbContext _db;
        private readonly IPatientService _patientService;

        public AllotmentService(HmsDbContext db, IPatientService patientService)
        {
            _db = db;
            _patientService = patientService;
        }

        public async Task<List<AllotmentViewModel>> GetPatientsByDepartment(int deptId)
        {
            var param = new SqlParameter("@DeptId", deptId);

            var data = await _db.AllotmentDbModels
                .FromSqlRaw("EXEC SP_GetPatientsByDepartment @DeptId", param)
                .ToListAsync();

            return data.Select(x => new AllotmentViewModel
            {
                PatientId = x.PatientId,
                OpNo = x.OpNo,
                PatientName = x.PatientName,
                CategoryName = x.CategoryName,
                FromDepartment = x.FromDepartment,
                FromDeptId = x.FromDeptId,
                ToDeptId = x.ToDeptId,
                AllotmentStatus = x.AllotmentStatus,
                FromDate = x.FromDate
            }).ToList();
        }

        public async Task<StudentAllotmentViewModel> GetAllotFormData(int patientId, int deptId)
        {
            var patient = await _patientService.GetPatientDetails(patientId);

            var referralTask = _db.ReferralStatuses
                .Where(x => x.PatientId == patientId && x.ToDeptId == deptId)
                .Select(x => x.ReferredId)
                .FirstOrDefaultAsync();

            var studentsTask = _db.Students
                .Where(x => x.DepartmentId == deptId)
                .ToListAsync();

            var doctorsTask = _db.Doctors
                .Where(x => x.DepartmentId == deptId)
                .ToListAsync();

            await Task.WhenAll(referralTask, studentsTask, doctorsTask);

            return new StudentAllotmentViewModel
            {
                PatientId = patientId,
                DeptId = deptId,

                // 👇 PATIENT DETAILS COMES FROM PATIENT SERVICE
                PatientName = patient?.PatientName,
                // you can also add Age, UHID etc later

                ReferredId = await referralTask,
                Students = await studentsTask,
                Doctors = await doctorsTask
            };
        }

        public async Task SaveAllotment(StudentAllotmentViewModel model)
        {
            var entity = new StudentAllotment
            {
                PatientId = model.PatientId,
                DeptId = model.DeptId,
                StudentId = model.StudentId,
                ReferredId = model.ReferredId,
                DoctorId = model.DoctorId,
                AllotDate = DateTime.Now,
                CreatedDate = DateTime.Now
            };

            _db.StudentAllotments.Add(entity);

            var referral = await _db.ReferralStatuses
        .FirstOrDefaultAsync(x =>  x.ReferredId ==  model.ReferredId);

            if (referral != null)
            {
                referral.AllotmentStatus = "Allotted";

                _db.ReferralStatuses.Update(referral);
            }

            await _db.SaveChangesAsync();
        }

    }
}
