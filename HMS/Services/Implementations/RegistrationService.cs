using HMS.Data;
using HMS.Entities;
using HMS.Models;
using HMS.Services.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace HMS.Services.Implementations
{
    public class RegistrationService : IRegistrationService
    {
        private readonly HmsDbContext _context;
        private static readonly object _uhidLock = new();
        private static readonly object _opLock = new();

        public RegistrationService(HmsDbContext context)
        {
            _context = context;
        }

        // ---------------- GET ALL ----------------
        public async Task<List<OPDPatientRegistration>> GetAllAsync()
        {
            return await _context.OPDPatientRegistrations
                .OrderByDescending(x => x.PatientId)
                .ToListAsync();
        }

        // ---------------- GET BY ID ----------------
        public async Task<OPDPatientRegistration?> GetByIdAsync(long id)
        {
            return await _context.OPDPatientRegistrations
                .FirstOrDefaultAsync(x => x.PatientId == id);
        }

        public async Task<OPDPatientRegistration?> GetForDeleteAsync(long id)
            => await GetByIdAsync(id);

        // ---------------- BUILD CREATE ----------------
        public async Task<RegistrationFormViewModel> BuildCreateAsync()
        {
            return await BuildFormAsync(new OPDPatientRegistration
            {
                RegDate = DateTime.Now,
                IsRegCancelled = false
            });
        }

        // ---------------- BUILD EDIT ----------------
        public async Task<RegistrationFormViewModel?> BuildEditAsync(long id)
        {
            var patient = await GetByIdAsync(id);
            return patient == null ? null : await BuildFormAsync(patient);
        }

        // ---------------- FORM BUILDER ----------------
        public async Task<RegistrationFormViewModel> BuildFormAsync(OPDPatientRegistration patient)
        {
            return new RegistrationFormViewModel
            {
                Patient = patient,

                Categories = await _context.MASCategory
                    .Where(x => x.IsActive)
                    .OrderBy(x => x.CategoryName)
                    .ToListAsync(),

                Countries = await _context.MASCountries
                    .Where(x => x.IsActive)
                    .OrderBy(x => x.CountryName)
                    .ToListAsync(),

                PaymentModes = await _context.MASPaymentCodes
                    .Where(x => x.IsActive)
                    .OrderBy(x => x.PaymodeName)
                    .ToListAsync(),

                States = patient.CountryId.HasValue
                    ? await _context.MASStates
                        .Where(x => x.CountryId == patient.CountryId && x.IsActive)
                        .OrderBy(x => x.StateName)
                        .ToListAsync()
                    : new List<MASState>(),

                Mandals = patient.StateId.HasValue
                    ? await _context.MASMandals
                        .Where(x => x.StateId == patient.StateId && x.IsActive)
                        .OrderBy(x => x.MandalName)
                        .ToListAsync()
                    : new List<MASMandal>(),

                Cities = patient.MandalId.HasValue
                    ? await _context.MASCities
                        .Where(x => x.MandalId == patient.MandalId && x.IsActive)
                        .OrderBy(x => x.CityName)
                        .ToListAsync()
                    : new List<MASCity>(),

                Titles = new List<string>
        {
            "Mr",
            "Mrs",
            "Miss",
            "Master",
            "Baby",
            "Dr"
        }
            };
        }

        // ---------------- CREATE ----------------
        public async Task<long> CreateAsync(OPDPatientRegistration patient)
        {
            patient.RegDate = DateTime.Now;
            patient.IsRegCancelled = false;
            patient.UHID = await GenerateUHIDAsync();
            patient.OpNo = await GenerateOpNoAsync();

            _context.OPDPatientRegistrations.Add(patient);
            await _context.SaveChangesAsync();
            return patient.PatientId;
        }

        // ---------------- UPDATE ----------------
        public async Task<bool> UpdateAsync(OPDPatientRegistration patient)
        {
            var existing = await _context.OPDPatientRegistrations
                .FirstOrDefaultAsync(x => x.PatientId == patient.PatientId);

            if (existing == null)
                return false;

            // ---------------- UPDATE FIELDS ----------------
            existing.Title = patient.Title;
            existing.PatientName = patient.PatientName;
            existing.FatherOrHusband = patient.FatherOrHusband;
            existing.Dob = patient.Dob;
            existing.Gender = patient.Gender;
            existing.Phone = patient.Phone;
            existing.AadharNo = patient.AadharNo;

            existing.Address = patient.Address;
            existing.CountryId = patient.CountryId;
            existing.StateId = patient.StateId;
            existing.MandalId = patient.MandalId;
            existing.CityId = patient.CityId;

            existing.CategoryId = patient.CategoryId;
            existing.PaymodeId = patient.PaymodeId;

            existing.TotalAmount = patient.TotalAmount;
            existing.DiscountPer = patient.DiscountPer;
            existing.NetAmount = patient.NetAmount;
            existing.PaidAmount = patient.PaidAmount;

            await _context.SaveChangesAsync();
            return true;
        }

        // ---------------- DELETE ----------------
        public async Task<bool> DeleteAsync(long id)
        {
            var data = await _context.OPDPatientRegistrations.FindAsync(id);
            if (data == null) return false;

            _context.OPDPatientRegistrations.Remove(data);
            await _context.SaveChangesAsync();
            return true;
        }

        // ---------------- DUPLICATE CHECK ----------------
        public async Task<bool> IsDuplicateAsync(string phone, string aadharNo)
        {
            return await _context.OPDPatientRegistrations
                .AnyAsync(x =>
                    (!string.IsNullOrEmpty(phone) && x.Phone == phone) ||
                    (!string.IsNullOrEmpty(aadharNo) && x.AadharNo == aadharNo));
        }

        // ---------------- LOCATION ----------------
        public async Task<List<MASState>> GetStatesByCountryAsync(int countryId)
        {
            return await _context.MASStates
                .Where(x => x.CountryId == countryId && x.IsActive)
                .ToListAsync();
        }

        public async Task<List<MASMandal>> GetMandalsByStateAsync(int stateId)
        {
            return await _context.MASMandals
                .Where(x => x.StateId == stateId && x.IsActive)
                .ToListAsync();
        }

        public async Task<List<MASCity>> GetCitiesByMandalAsync(int mandalId)
        {
            return await _context.MASCities
                .Where(x => x.MandalId == mandalId && x.IsActive)
                .ToListAsync();
        }

        public Task<string> GenerateUHIDAsync()
        { 
            lock (_uhidLock)
            {
                using var con = new SqlConnection(_context.Database.GetConnectionString());
                con.Open();

                using var tran = con.BeginTransaction();

                try
                {
                    string query = @"
                SELECT LastNumber 
                FROM MASNumberSeries WITH (UPDLOCK, ROWLOCK)
                WHERE SeriesName = 'UHID'";

                    long lastNo;

                    using (var cmd = new SqlCommand(query, con, tran))
                    {
                        lastNo = Convert.ToInt64(cmd.ExecuteScalar());
                    }

                    long newNo = lastNo + 1;

                    string uhid = $"UHID{DateTime.Now.Year}{newNo.ToString("D6")}";

                    string update = @"
                UPDATE MASNumberSeries
                SET LastNumber = @LastNumber,
                    UpdatedDate = GETDATE()
                WHERE SeriesName = 'UHID'";

                    using (var cmd = new SqlCommand(update, con, tran))
                    {
                        cmd.Parameters.AddWithValue("@LastNumber", newNo);
                        cmd.ExecuteNonQuery();
                    }

                    tran.Commit();
                   return Task.FromResult(uhid);
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
        }


        public Task<string> GenerateOpNoAsync()
        {
            lock (_opLock)
            {
                using var con = new SqlConnection(_context.Database.GetConnectionString());
                con.Open();

                using var tran = con.BeginTransaction();

                try
                {
                    string todayKey = DateTime.Now.ToString("yyyyMMdd");

                    // check last number for today
                    string query = @"
                SELECT LastNumber 
                FROM MASNumberSeries WITH (UPDLOCK, ROWLOCK)
                WHERE SeriesName = 'OPNO'";

                    long lastNo;

                    using (var cmd = new SqlCommand(query, con, tran))
                    {
                        lastNo = Convert.ToInt64(cmd.ExecuteScalar());
                    }

                    long newNo = lastNo + 1;

                    string opNo = $"OP{todayKey}-{newNo.ToString("D4")}";

                    string update = @"
                UPDATE MASNumberSeries
                SET LastNumber = @LastNumber,
                    UpdatedDate = GETDATE()
                WHERE SeriesName = 'OPNO'";

                    using (var cmd = new SqlCommand(update, con, tran))
                    {
                        cmd.Parameters.AddWithValue("@LastNumber", newNo);
                        cmd.ExecuteNonQuery();
                    }

                    tran.Commit();
                    return Task.FromResult(opNo);
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
        }

        public async Task<OPDSearchViewModel> SearchOPDAsync(OPDSearchViewModel model)
        {
            var query = _context.OPDPatientRegistrations.AsQueryable();

            // ---------------- FILTERS ----------------

            if (!string.IsNullOrEmpty(model.OPNo))
                query = query.Where(x => x.OpNo.Contains(model.OPNo));

            if (!string.IsNullOrEmpty(model.Phone))
                query = query.Where(x => x.Phone.Contains(model.Phone));

            if (!string.IsNullOrEmpty(model.PatientName))
                query = query.Where(x => x.PatientName.Contains(model.PatientName));

            if (!string.IsNullOrEmpty(model.AadharNo))
                query = query.Where(x => x.AadharNo.Contains(model.AadharNo));

            if (model.FromDate.HasValue)
                query = query.Where(x => x.RegDate >= model.FromDate);

            if (model.ToDate.HasValue)
                query = query.Where(x => x.RegDate <= model.ToDate);

            // ---------------- TOTAL COUNT ----------------
            model.TotalRecords = await query.CountAsync();

            // ---------------- PAGINATION ----------------
            model.Results = await query
                .OrderByDescending(x => x.PatientId)
                .Skip((model.PageNumber - 1) * model.PageSize)
                .Take(model.PageSize)
                .ToListAsync();

            return model;
        }
        public async Task<MASHospital?> GetHospitalAsync()
        {
            return await _context.MASHospitals
                .FirstOrDefaultAsync(x => x.IsActive);
        }
    }
}