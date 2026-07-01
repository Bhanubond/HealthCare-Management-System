using HMS.Data;
using HMS.Services;
using HMS.Services.Implementations;
using HMS.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews(options =>
{
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
});
builder.Services.AddDbContext<HmsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("VmsDb")));
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IMASDepartmentService, MASDepartmentService>();
builder.Services.AddScoped<IScreenService, ScreenService>();
builder.Services.AddScoped<IMASCourseService, MASCourseService>();
builder.Services.AddScoped<IMASCourseYearService, MASCourseYearService>();
builder.Services.AddScoped<IMASDesignationService, MASDesignationService>();
builder.Services.AddScoped<IMASCountryService, MASCountryService>();
builder.Services.AddScoped<IMASStateService, MASStateService>();
builder.Services.AddScoped<IMASMandalService, MASMandalService>();
builder.Services.AddScoped<IMASCityService, MASCityService>();
builder.Services.AddScoped<IMASDeptScreenMappingService, MASDeptScreenMappingService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IRegistrationService, RegistrationService>();
builder.Services.AddScoped<IAllotmentService, AllotmentService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IGeneralMedicineServices, GeneralMedicineServices>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();

// REQUIRED for Session
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Session must be before Authorization if used in auth logic
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}"
);

app.Run();
