using HMS.Entities;
using HMS.Models;

namespace HMS.Services.Interfaces
{
    public interface IMASCourseService
    {
        Task<List<CourseDetailsDto>> GetAllAsync();

        Task<CourseDetailsDto?> GetByIdAsync(int id);

        Task<CourseDetailsDto?> GetDetailsAsync(int id);

        Task<CourseDetailsDto?> GetForDeleteAsync(int id);

        Task<bool> CreateAsync(MASCourse course);

        Task<bool> UpdateAsync(MASCourse course);

        Task<bool> DeleteAsync(int id);
    }
}