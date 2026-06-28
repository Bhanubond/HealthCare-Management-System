using HMS.Entities;
using HMS.Models;

namespace HMS.Services.Interfaces
{
    public interface IMASCourseYearService
    {
        Task<List<MASCourseYear>> GetAllAsync();
        Task<CourseYearDto?> GetByIdAsync(int id);
        Task<List<MASCourse>> GetActiveCoursesAsync();
        Task<bool> CreateAsync(MASCourseYear year);
        Task<bool> UpdateAsync(MASCourseYear year);
        Task<bool> DeleteAsync(int id);
    }
}
