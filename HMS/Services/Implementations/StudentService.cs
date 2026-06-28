using HMS.Data;
using HMS.Entities;
using HMS.Models;
using HMS.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HMS.Services.Implementations
{
    public class StudentService : IStudentService
    {
        private readonly HmsDbContext _context;

        public StudentService(HmsDbContext context)
        {
            _context = context;
        }

        public async Task<List<Student>> GetAllAsync()
        {
            return await _context.Students
                .Include(x => x.Course)
                .Include(x => x.CourseYear)
                .Include(x => x.Department)
                .Include(x => x.Country)
                .Include(x => x.State)
                .Include(x => x.Mandal)
                .Include(x => x.City)
                .OrderByDescending(x => x.StudentId)
                .ToListAsync();
        }
        public async Task<Student?> GetByIdAsync(long id)
        {
            return await _context.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.StudentId == id);
        }

        public async Task<StudentDetailsVm?> GetDetailsAsync(long id)
        {
            return await (from s in _context.Students

                          join c in _context.MASCourses
                              on s.CourseId equals c.CourseId into c1
                          from c in c1.DefaultIfEmpty()

                          join y in _context.MASCourseYears
                              on s.CourseYearId equals y.CourseYearId into y1
                          from y in y1.DefaultIfEmpty()

                          join d in _context.MASDepartments
                              on s.DepartmentId equals d.DeptId into d1
                          from d in d1.DefaultIfEmpty()

                          join co in _context.MASCountries
                              on s.CountryId equals co.CountryId into co1
                          from co in co1.DefaultIfEmpty()

                          join st in _context.MASStates
                              on s.StateId equals st.StateId into st1
                          from st in st1.DefaultIfEmpty()

                          join m in _context.MASMandals
                              on s.MandalId equals m.MandalId into m1
                          from m in m1.DefaultIfEmpty()

                          join ci in _context.MASCities
                              on s.CityId equals ci.CityId into ci1
                          from ci in ci1.DefaultIfEmpty()

                          where s.StudentId == id

                          select new StudentDetailsVm
                          {
                              StudentId = s.StudentId,
                              StudentName = s.StudentName,
                              StudentCode = s.StudentCode,
                              Gender = s.Gender,
                              Phone = s.Phone,
                              Email = s.Email,
                              Address = s.Address,
                              DOB = s.DOB,

                              CourseName = c.CourseName,
                              YearName = y.YearName,
                              DeptName = d.DeptName,
                              CountryName = co.CountryName,
                              StateName = st.StateName,
                              MandalName = m.MandalName,
                              CityName = ci.CityName,

                              AdmissionDate = s.AdmissionDate,
                              IsActive = s.IsActive
                          }).FirstOrDefaultAsync();
        }

        public async Task<Student?> GetForDeleteAsync(long id) => await GetByIdAsync(id);

        public async Task<StudentFormViewModel> BuildCreateAsync()
        {
            return await BuildFormAsync(new Student
            {
                AdmissionDate = DateTime.Now,
                IsActive = true
            });
        }

        public async Task<StudentFormViewModel?> BuildEditAsync(long id)
        {
            var student = await GetByIdAsync(id);

            if (student == null)
                return null;

            // ensure safe object mapping (no need to re-create manually unless required)
            var model = new Student
            {
                StudentId = student.StudentId,
                StudentCode = student.StudentCode,
                StudentName = student.StudentName,

                Gender = student.Gender,
                DOB = student.DOB,

                Phone = student.Phone,
                Email = student.Email,
                Address = student.Address,

                CourseId = student.CourseId,
                CourseYearId = student.CourseYearId,
                DepartmentId = student.DepartmentId,

                CountryId = student.CountryId,
                StateId = student.StateId,
                MandalId = student.MandalId,
                CityId = student.CityId,

                AdmissionDate = student.AdmissionDate,
                IsActive = student.IsActive
            };

            return await BuildFormAsync(model);
        }

        public async Task<StudentFormViewModel> BuildFormAsync(Student student)
        {
            return new StudentFormViewModel
            {
                Student = student,

                Courses = await _context.MASCourses.Where(x => x.IsActive).ToListAsync(),

                CourseYears = student.CourseId.HasValue
                ? await _context.MASCourseYears
                    .Where(x => x.IsActive && x.CourseId == student.CourseId)
                    .OrderBy(x => x.YearName)
                    .ToListAsync()
                : new List<MASCourseYear>(),

                Departments = await _context.MASDepartments.Where(x => x.IsActive).ToListAsync(),

                Countries = await _context.MASCountries.Where(x => x.IsActive).ToListAsync(),


                States = student.CountryId.HasValue
                    ? await _context.MASStates
                        .Where(x => x.IsActive && x.CountryId == student.CountryId)
                        .ToListAsync()
                    : new List<MASState>(),

                Mandals = student.StateId.HasValue
                    ? await _context.MASMandals
                        .Where(x => x.IsActive && x.StateId == student.StateId)
                        .ToListAsync()
                    : new List<MASMandal>(),

                Cities = student.MandalId.HasValue
                    ? await _context.MASCities
                        .Where(x => x.IsActive && x.MandalId == student.MandalId)
                        .ToListAsync()
                    : new List<MASCity>()
            };
        }

        public async Task<bool> CreateAsync(Student student)
        {
            student.AdmissionDate = student.AdmissionDate == default ? DateTime.Now : student.AdmissionDate;
            student.IsActive = true;
            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(Student student)
        {
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> DeleteAsync(long id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null) return false;
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<MASCourseYear>> GetCourseYearsByCourseAsync(int courseId)
        {
            return await _context.MASCourseYears.Where(x => x.CourseId == courseId && x.IsActive)
                .OrderBy(x => x.YearName).ToListAsync();
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
