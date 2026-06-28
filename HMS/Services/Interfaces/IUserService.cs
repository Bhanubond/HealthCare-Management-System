using HMS.Models;

namespace HMS.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<UserIndexViewModel>> GetAllAsync();

        Task<UserDetailsDto?> GetDetailsAsync(int id);

        Task<UserDetailsDto?> GetForDeleteAsync(int id);

        Task<UserFormViewModel> BuildCreateAsync();

        Task<UserFormViewModel?> BuildEditAsync(int id);

        Task<UserFormViewModel> BuildFormAsync(UserFormViewModel model);

        Task<bool> CreateAsync(UserFormViewModel model);

        Task<bool> UpdateAsync(UserFormViewModel model);

        Task<bool> DeleteAsync(int id);
    }
}
