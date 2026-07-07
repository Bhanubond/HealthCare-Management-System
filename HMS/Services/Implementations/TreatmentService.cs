using HMS.Common;
using HMS.Data;
using HMS.Models;
using HMS.Services.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace HMS.Services.Implementations
{
    public class TreatmentService : ITreatmentService
    {
        private readonly HmsDbContext _db;
        private readonly IGeneralMedicineServices _gmService;
        private readonly IEmergencyService _emrservice;

        public TreatmentService(HmsDbContext db, IGeneralMedicineServices gmService, IEmergencyService emrservice)
        {
            _db = db;
            _gmService = gmService;
            _emrservice = emrservice;
        }

        public async Task<List<TreatmentQueueVm>> GetTreatmentQueue(int deptId, DateTime fromDate, DateTime toDate)
        {
            var parameters = new[]
            {
                new SqlParameter("@DeptId", deptId),
                new SqlParameter("@FromDate", fromDate),
                new SqlParameter("@ToDate", toDate)
            };

            return await _db.Set<TreatmentQueueVm>()
                .FromSqlRaw(
                    "EXEC usp_GetTreatmentQueue @DeptId,@FromDate,@ToDate",
                    parameters)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<object> GetTreatmentScreenAsync(int deptId, int patientId)
        {
            var dept = (Department)deptId;

            return dept switch
            {
                Department.GEN => await _gmService.GetTreatmentScreenAsync(deptId, patientId),
                Department.EMR => await _emrservice.GetTreatmentScreenAsync(deptId, patientId),
                //Department.PED => await _pediatricsService.GetTreatmentScreenAsync(patientId),
                //Department.ORT => await _orthoService.GetTreatmentScreenAsync(patientId),

                _ => throw new Exception($"Invalid Department: {dept}")
            };
        }
    }
}