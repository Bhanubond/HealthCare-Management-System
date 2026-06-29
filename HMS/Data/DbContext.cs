using Microsoft.EntityFrameworkCore;
using HMS.Entities;
using HMS.Models;
namespace HMS.Data
{
    public class HmsDbContext : DbContext
    {
        public HmsDbContext(DbContextOptions<HmsDbContext> options) : base(options) { }

        public DbSet<Users> Users { get; set; }
        public DbSet<UserLoginLog> UserLoginLogs { get; set; }
        public DbSet<MASCategory> MASCategory { get; set; }
        public DbSet<MASCourse> MASCourses { get; set; }
        public DbSet<MASCourseYear> MASCourseYears { get; set; }
        public DbSet<MASDesignation> MASDesignations { get; set; }
        public DbSet<MASCountry> MASCountries { get; set; }
        public DbSet<MASState> MASStates { get; set; }
        public DbSet<MASMandal> MASMandals { get; set; }
        public DbSet<MASCity> MASCities { get; set; }
        public DbSet<MASDepartment> MASDepartments { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<MASScreen> MASScreens { get; set; }
        public DbSet<MASDeptScreenMapping> MASDeptScreenMappings { get; set; }
        public DbSet<OPDPatientRegistration> OPDPatientRegistrations { get; set; }
        public DbSet<MASPaymentCode> MASPaymentCodes { get; set; }
        public DbSet<MASHospital> MASHospitals { get; set; }
        public DbSet<ReferralStatus> ReferralStatuses { get; set; }
        public DbSet<StudentAllotment> StudentAllotments { get; set; }

        public DbSet<AllotmentDbModel> AllotmentDbModels { get; set; }

        public DbSet<PatientDetailsViewModel> PatientDetailsViewModel { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AllotmentDbModel>().HasNoKey().ToView(null);
            modelBuilder.Entity<PatientDetailsViewModel>().HasNoKey();
        }

    }
}