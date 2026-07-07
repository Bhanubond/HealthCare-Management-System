using HMS.Data;
using HMS.Entities;
using HMS.Models;
using HMS.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HMS.Services.Implementations
{
    public class FollowUpService : IFollowUpService
    {
        private readonly HmsDbContext _db;

        public FollowUpService(HmsDbContext db)
        {
            _db = db;
        }

        public async Task<FollowUp?> GetLatestFollowUpAsync(int patientId)
        {
            return await _db.FollowUps
                .AsNoTracking()
                .Where(x => x.PatientId == patientId)
                .OrderByDescending(x => x.FollowupId)
                .FirstOrDefaultAsync();
        }

        public async Task<int?> GetCurrentDepartmentIdAsync(int patientId)
        {
            return await _db.StudentAllotments
                .AsNoTracking()
                .Where(x => x.PatientId == patientId && x.DeptId.HasValue)
                .OrderByDescending(x => x.AllotId)
                .Select(x => x.DeptId)
                .FirstOrDefaultAsync();
        }

        //public async Task SaveOrUpdateFollowUpAsync(FollowUpSaveVm model)
        //{
        //    if (!model.NextVisitDate.HasValue)
        //        return;

        //    var now = DateTime.Now;

        //    var deptId = model.NextVisitDepartmentId.GetValueOrDefault();

        //    if (deptId <= 0)
        //    {
        //        deptId = await GetCurrentDepartmentIdAsync(model.PatientId) ?? 0;
        //    }

        //    if (deptId <= 0)
        //        return;

        //    var reason = model.NextVisitReason ?? model.FollowUpNotes;
        //    var doctorId = model.NextVisitDoctorId ?? model.DoctorId;
        //    var studentId = model.NextVisitStudentId ?? model.StudentId;

        //    var latest = await _db.FollowUps
        //        .Where(x => x.PatientId == model.PatientId)
        //        .OrderByDescending(x => x.FollowupId)
        //        .FirstOrDefaultAsync();

        //    if (latest == null)
        //    {
        //        _db.FollowUps.Add(new FollowUp
        //        {
        //            PatientId = model.PatientId,
        //            FollowupDate = model.NextVisitDate.Value,
        //            FollowupTime = model.NextVisitTime,
        //            DeptId = deptId,
        //            FollowupReason = reason,
        //            DoctorId = doctorId,
        //            StudentId = studentId,
        //            Status = model.Status ?? "Yet to visit",
        //            //ReferredTreatmentId = model.ReferredId.GetValueOrDefault(),
        //            CreatedDate = now,
        //            CreatedBy = "System",
        //            CreatedSystem = Environment.MachineName,
        //            IsCancelled = false
        //        });
        //    }
        //    else
        //    {
        //        latest.FollowupDate = model.NextVisitDate.Value;
        //        latest.FollowupTime = model.NextVisitTime;
        //        latest.DeptId = deptId;
        //        latest.FollowupReason = reason;
        //        latest.DoctorId = doctorId;
        //        latest.StudentId = studentId;
        //        latest.Status = model.Status ?? latest.Status;
        //        //latest.ReferredTreatmentId = model.ReferredId.GetValueOrDefault();
        //        latest.ModifiedDate = now;
        //        latest.ModifiedBy = "System";
        //        latest.ModifiedSystem = Environment.MachineName;
        //    }

        //    //await _db.SaveChangesAsync();
        //}

        public async Task SaveOrUpdateFollowUpAsync(FollowUpSaveVm model)
        {
            if (!model.FollowupDate.HasValue)
                return;

            var now = DateTime.Now;

            var deptId = model.DeptId;

            if (deptId <= 0)
            {
                deptId = await GetCurrentDepartmentIdAsync(model.PatientId) ?? 0;
            }

            if (deptId <= 0)
                return;

            var latest = await _db.FollowUps
                .Where(x => x.PatientId == model.PatientId)
                .OrderByDescending(x => x.FollowupId)
                .FirstOrDefaultAsync();

            if (latest == null)
            {
                _db.FollowUps.Add(new FollowUp
                {
                    PatientId = model.PatientId,

                    FollowupDate = model.FollowupDate.Value,

                    FollowupTime = model.FollowupTime,

                    DeptId = deptId,

                    FollowupReason = model.FollowupReason,

                    DoctorId = model.DoctorId,

                    StudentId = model.StudentId,

                    Status = model.Status,

                    ReferredTreatmentId = model.ReferredTreatmentId,

                    CreatedDate = now,

                    CreatedBy = "System",

                    CreatedSystem = Environment.MachineName,

                    IsCancelled = false
                });
            }
            else
            {
                latest.FollowupDate = model.FollowupDate.Value;

                latest.FollowupTime = model.FollowupTime;

                latest.DeptId = deptId;

                latest.FollowupReason = model.FollowupReason;

                latest.DoctorId = model.DoctorId;

                latest.StudentId = model.StudentId;

                latest.Status = model.Status;

                latest.ReferredTreatmentId = model.ReferredTreatmentId;

                latest.ModifiedDate = now;

                latest.ModifiedBy = "System";

                latest.ModifiedSystem = Environment.MachineName;
            }
        }
    }
}