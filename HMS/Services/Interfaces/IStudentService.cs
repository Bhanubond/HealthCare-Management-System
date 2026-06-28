using HMS.Models;
using HMS.Entities;

namespace HMS.Services.Interfaces
{
    public interface IStudentService
    {
        Task<List<Student>> GetAllAsync();
        Task<Student?> GetForDeleteAsync(long id);
        Task<StudentFormViewModel> BuildCreateAsync();
        Task<StudentFormViewModel?> BuildEditAsync(long id);
        Task<StudentFormViewModel> BuildFormAsync(Student student);
        Task<bool> CreateAsync(Student student);
        Task<bool> UpdateAsync(Student student);
        Task<bool> DeleteAsync(long id);
        Task<List<MASCourseYear>> GetCourseYearsByCourseAsync(int courseId);
        Task<List<MASState>> GetStatesByCountryAsync(int countryId);
        Task<List<MASMandal>> GetMandalsByStateAsync(int stateId);
        Task<List<MASCity>> GetCitiesByMandalAsync(int mandalId);
        Task<StudentDetailsVm?> GetDetailsAsync(long id);
        Task<Student?> GetByIdAsync(long id);
    }
}
