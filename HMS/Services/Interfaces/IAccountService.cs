using HMS.Entities;
using HMS.Models;

namespace HMS.Services.Interfaces
{
    public interface IAccountService
    {
        Task<Users?> AuthenticateAsync(LoginViewModel model);
        Task<bool> IsUserNameTakenAsync(string userName);
        Task<Users> RegisterAsync(RegisterViewModel model);
        Task RecordLoginAsync(Users user, string? ipAddress, string? browserName, string? sessionId);
        Task RecordLogoutAsync(int userId);
    }
}
