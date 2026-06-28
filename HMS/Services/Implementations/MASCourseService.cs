using HMS.Data;
using HMS.Entities;
using HMS.Models;
using HMS.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace HMS.Services.Implementations
{
    public class MASCourseService : IMASCourseService
    {
        private readonly HmsDbContext _context;

        public MASCourseService(HmsDbContext context)
        {
            _context = context;
        }

        public async Task<List<CourseDetailsDto>> GetAllAsync()
        {
            return await _context.MASCourses
                 .Select(c => new CourseDetailsDto
                 {
                     CourseId = c.CourseId,
                     CourseName = c.CourseName,
                     IsActive = c.IsActive
                 })
                 .ToListAsync();
        }

        public async Task<CourseDetailsDto?> GetByIdAsync(int id)
        {
            return await _context.MASCourses
                .Where(c => c.CourseId == id)
                .Select(c => new CourseDetailsDto
                {
                    CourseId = c.CourseId,
                    CourseName = c.CourseName,
                    IsActive = c.IsActive
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> CreateAsync(MASCourse course)
        {
            _context.MASCourses.Add(course);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(MASCourse course)
        {
            _context.MASCourses.Update(course);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var course = await _context.MASCourses.FindAsync(id);
            if (course == null) return false;
            _context.MASCourses.Remove(course);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<CourseDetailsDto?> GetDetailsAsync(int id)
        {
            return await _context.MASCourses
                .Where(x => x.CourseId == id)
                .Select(x => new CourseDetailsDto
                {
                    CourseId = x.CourseId,
                    CourseName = x.CourseName,
                    IsActive = x.IsActive
                })
                .FirstOrDefaultAsync();
        }

        public async Task<CourseDetailsDto?> GetForDeleteAsync(int id)
        {
            return await GetDetailsAsync(id);
        }
    }
}
