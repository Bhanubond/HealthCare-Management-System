using HMS.Data;
using HMS.Entities;
using HMS.Models;
using HMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HMS.Services.Implementations
{
    public class AppointmentService : IAppointmentService
    {
        private readonly HmsDbContext _db;

        public AppointmentService(HmsDbContext db)
        {
            _db = db;
        }

        public async Task SaveFollowUpAsync(FollowUpVm model)
        {
            var entity = new FollowUp
            {
                PatientId = model.PatientId,
                FollowupDate = model.FollowupDate,
                FollowupTime = model.FollowupTime,
                DeptId = model.DeptId,
                FollowupReason = model.FollowupReason,
                DoctorId = model.DoctorId,
                StudentId = model.StudentId,
                RevisitId = model.RevisitId,
                Status = model.Status ?? "Yet to visit",
                IgnoreReason = model.IgnoreReason,
                ReferredTreatmentId = model.ReferredTreatmentId,
                CreatedDate = DateTime.Now,
                CreatedBy = "System",
                CreatedSystem = Environment.MachineName,
                IsCancelled = false
            };

            _db.FollowUps.Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateFollowUpAsync(FollowUpVm model)
        {
            var entity = await _db.FollowUps
                .FirstOrDefaultAsync(x => x.FollowupId == model.FollowupId);

            if (entity == null)
                return;

            entity.FollowupDate = model.FollowupDate;
            entity.FollowupTime = model.FollowupTime;
            entity.DeptId = model.DeptId;
            entity.FollowupReason = model.FollowupReason;
            entity.DoctorId = model.DoctorId;
            entity.StudentId = model.StudentId;
            entity.Status = model.Status;
            entity.IgnoreReason = model.IgnoreReason;
            entity.RevisitId = model.RevisitId;
            entity.ReferredTreatmentId = model.ReferredTreatmentId;

            entity.ModifiedDate = DateTime.Now;
            entity.ModifiedBy = "System";
            entity.ModifiedSystem = Environment.MachineName;

            await _db.SaveChangesAsync();
        }

        public async Task<FollowUpVm?> GetFollowUpAsync(int followUpId)
        {
            return await _db.FollowUps
                .Where(x => x.FollowupId == followUpId)
                .Select(x => new FollowUpVm
                {
                    FollowupId = x.FollowupId,
                    PatientId = x.PatientId,
                    FollowupDate = x.FollowupDate,
                    FollowupTime = x.FollowupTime,
                    DeptId = x.DeptId,
                    FollowupReason = x.FollowupReason,
                    DoctorId = x.DoctorId,
                    StudentId = x.StudentId,
                    Status = x.Status,
                    IgnoreReason = x.IgnoreReason,
                    RevisitId = x.RevisitId,
                    ReferredTreatmentId = x.ReferredTreatmentId
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<SelectListItem>> GetDoctorOptionsAsync()
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

        public async Task<List<SelectListItem>> GetStudentOptionsAsync()
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
    }
}
