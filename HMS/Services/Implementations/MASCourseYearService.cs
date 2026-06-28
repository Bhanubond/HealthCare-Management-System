using HMS.Data;
using HMS.Entities;
using HMS.Models;
using HMS.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HMS.Services.Implementations
{
    public class MASCourseYearService : IMASCourseYearService
    {
        private readonly HmsDbContext _context;

        public MASCourseYearService(HmsDbContext context)
        {
            _context = context;
        }

        public Task<List<MASCourseYear>> GetAllAsync()
     => _context.MASCourseYears
         .Include(x => x.Course)
         .OrderBy(x => x.CourseId)
         .ToListAsync();

        public async Task<CourseYearDto?> GetByIdAsync(int id)
        {
            return await (
                from cy in _context.MASCourseYears
                join c in _context.MASCourses
                    on cy.CourseId equals c.CourseId

                where cy.CourseYearId == id

                select new CourseYearDto
                {
                    CourseYearId = cy.CourseYearId,
                    CourseName = c.CourseName,
                    YearName = cy.YearName,
                    IsActive = cy.IsActive
                }
            ).FirstOrDefaultAsync();
        }

        public Task<List<MASCourse>> GetActiveCoursesAsync()
            => _context.MASCourses.Where(x => x.IsActive).OrderBy(x => x.CourseName).ToListAsync();

        public async Task<bool> CreateAsync(MASCourseYear year)
        {
            _context.MASCourseYears.Add(year);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(MASCourseYear year)
        {
            _context.MASCourseYears.Update(year);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var year = await _context.MASCourseYears.FindAsync(id);
            if (year == null) return false;
            _context.MASCourseYears.Remove(year);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
