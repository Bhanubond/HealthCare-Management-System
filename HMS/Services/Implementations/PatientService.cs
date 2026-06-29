using HMS.Data;
using HMS.Models;
using HMS.Services.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace HMS.Services.Implementations
{
    public class PatientService : IPatientService
    {
        private readonly HmsDbContext _db;

        public PatientService(HmsDbContext db)
        {
            _db = db;
        }

        public async Task<PatientDetailsViewModel> GetPatientDetails(int patientId)
        {
            var param = new SqlParameter("@PatientId", patientId);

            var result = await _db.Set<PatientDetailsViewModel>()
                .FromSqlRaw("EXEC SP_GetPatientDetailsById @PatientId", param)
                .ToListAsync();

            return result.FirstOrDefault();
        }
    }
}
