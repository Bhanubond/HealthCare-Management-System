using HMS.Data;
using HMS.Entities;
using HMS.Models;
using HMS.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HMS.Services.Implementations
{
    public class ReferralStatusService : IReferralStatusService
    {
        private readonly HmsDbContext _db;

        public ReferralStatusService(HmsDbContext db)
        {
            _db = db;
        }

        public async Task CreateReferralsAsync(int patientId, int fromDeptId, List<int> toDeptIds, Dictionary<int, string> reasons)
        {
            foreach (var deptId in toDeptIds)
            {
                var referral = new ReferralStatus
                {
                    PatientId = patientId,
                    FromDeptId = fromDeptId,
                    ToDeptId = deptId,
                    //ReferredReason = reasons.ContainsKey(deptId) ? reasons[deptId] : null,

                    ReferredReason = reasons.ContainsKey(deptId) && !string.IsNullOrWhiteSpace(reasons[deptId]) ? reasons[deptId] : "Initial Examination",

                    FromDate = DateTime.Now,
                    TreatmentStatus = "Pending",
                    AllotmentStatus = "Pending",
                    CreatedDate = DateTime.Now
                };

                _db.ReferralStatuses.Add(referral);
            }

            await _db.SaveChangesAsync();
        }

        public async Task<List<ReferralStatus>> GetByPatientIdAsync(int patientId)
        {
            return await _db.ReferralStatuses
                .Where(x => x.PatientId == patientId)
                .ToListAsync();
        }

        public async Task UpdateStatusAsync(int referredId, string status)
        {
            var referral = await _db.ReferralStatuses
                .FirstOrDefaultAsync(x => x.ReferredId == referredId);

            if (referral != null)
            {
                referral.TreatmentStatus = status;
                referral.ModifiedDate = DateTime.Now;
                await _db.SaveChangesAsync();
            }
        }

        public async Task<List<ReferralStatus>> GetByDepartmentAsync(int deptId)
        {
            return await _db.ReferralStatuses
                .Where(x => x.ToDeptId == deptId)
                .ToListAsync();
        }

        public async Task<List<ReferralDisplayVm>> GetReferralsByPatientAsync(int patientId)
        {
            return await (from r in _db.ReferralStatuses
                          join d in _db.MASDepartments on r.ToDeptId equals d.DeptId
                          where r.PatientId == patientId
                          select new ReferralDisplayVm
                          {
                              DepartmentName = d.DeptName,
                              Reason = r.ReferredReason,
                              Status = r.TreatmentStatus,
                              Date = r.FromDate
                          }).ToListAsync();
        }

        public async Task CompleteReferralAsync(int referredId)
        {
            var referral = await _db.ReferralStatuses
                .FirstOrDefaultAsync(x => x.ReferredId == referredId);

            if (referral == null)
                return;

            referral.TreatmentStatus = "Completed";
            referral.TreatmentDate = DateTime.Now;
            referral.ModifiedDate = DateTime.Now;

            await _db.SaveChangesAsync();
        }
    }
}
