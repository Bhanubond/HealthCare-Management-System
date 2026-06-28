using HMS.Data;
using HMS.Entities;
using HMS.Models;
using HMS.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HMS.Services.Implementations
{
    public class DoctorService : IDoctorService
    {
        private readonly HmsDbContext _context;

        public DoctorService(HmsDbContext context)
        {
            _context = context;
        }

        public async Task<List<Doctor>> GetAllAsync()
        {
            return await _context.Doctors
                .Include(x => x.Department)
                .Include(x => x.Designation)
                .Include(x => x.Country)
                .Include(x => x.State)
                .Include(x => x.Mandal)
                .Include(x => x.City)
                .OrderByDescending(x => x.DoctorId)
                .ToListAsync();
        }

        public async Task<Doctor?> GetByIdAsync(long id)
        {
            return await _context.Doctors
                .Include(x => x.Department)
                .Include(x => x.Designation)
                .Include(x => x.Country)
                .Include(x => x.State)
                .Include(x => x.Mandal)
                .Include(x => x.City)
                .FirstOrDefaultAsync(x => x.DoctorId == id);
        }

        public async Task<Doctor?> GetForDeleteAsync(long id) => await GetByIdAsync(id);

        public async Task<DoctorFormViewModel> BuildCreateAsync()
        {
            return await BuildFormAsync(new Doctor
            {
                JoiningDate = DateTime.Today,
                IsActive = true
            });
        }

        public async Task<DoctorFormViewModel?> BuildEditAsync(long id)
        {
            var doctor = await GetByIdAsync(id);
            return doctor == null ? null : await BuildFormAsync(doctor);
        }

        public async Task<DoctorFormViewModel> BuildFormAsync(Doctor doctor)
        {
            return new DoctorFormViewModel
            {
                Doctor = doctor,

                Departments = await _context.MASDepartments.Where(x => x.IsActive).ToListAsync(),
                Designations = await _context.MASDesignations.Where(x => x.IsActive).ToListAsync(),
                Countries = await _context.MASCountries.Where(x => x.IsActive).ToListAsync(),

                // IMPORTANT: always empty
                States = new List<MASState>(),
                Mandals = new List<MASMandal>(),
                Cities = new List<MASCity>()
            };
        }

        public async Task<bool> CreateAsync(Doctor doctor)
        {
            doctor.JoiningDate = doctor.JoiningDate == default ? DateTime.Today : doctor.JoiningDate;
            doctor.IsActive = true;
            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(Doctor doctor)
        {
            _context.Doctors.Update(doctor);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null) return false;
            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<MASState>> GetStatesByCountryAsync(int countryId)
        {
            return await _context.MASStates.Where(x => x.CountryId == countryId && x.IsActive)
                .OrderBy(x => x.StateName).ToListAsync();
        }

        public async Task<List<MASMandal>> GetMandalsByStateAsync(int stateId)
        {
            return await _context.MASMandals.Where(x => x.StateId == stateId && x.IsActive)
                .OrderBy(x => x.MandalName).ToListAsync();
        }

        public async Task<List<MASCity>> GetCitiesByMandalAsync(int mandalId)
        {
            return await _context.MASCities.Where(x => x.MandalId == mandalId && x.IsActive)
                .OrderBy(x => x.CityName).ToListAsync();
        }
    }
}
