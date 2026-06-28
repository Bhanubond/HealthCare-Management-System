using HMS.Data;
using HMS.Entities;
using HMS.Models;
using HMS.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HMS.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly HmsDbContext _context;

        public AccountService(HmsDbContext context)
        {
            _context = context;
        }

        public async Task<Users?> AuthenticateAsync(LoginViewModel model)
        {
            return await _context.Users.FirstOrDefaultAsync(x =>
                x.UserName == model.UserName &&
                x.Password == model.Password &&
                x.Active);
        }

        public Task<bool> IsUserNameTakenAsync(string userName)
        {
            return _context.Users.AnyAsync(x => x.UserName == userName);
        }

        public async Task<Users> RegisterAsync(RegisterViewModel model)
        {
            var user = new Users
            {
                UserName = model.UserName,
                Password = model.Password,
                StudentId = model.StudentId,
                DoctorId = model.DoctorId,
                CategoryId = model.CategoryId,
                Active = true,
                PasswordReset = false,
                EnableLiveChat = false,
                IsOnline = false,
                CreatedDate = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task RecordLoginAsync(Users user, string? ipAddress, string? browserName, string? sessionId)
        {
            user.IsOnline = true;
            user.LastSeenAt = DateTime.Now;

            _context.UserLoginLogs.Add(new UserLoginLog
            {
                UserId = user.UserId,
                UserName = user.UserName,
                LoginTime = DateTime.Now,
                LoginStatus = "Success",
                IPAddress = ipAddress,
                BrowserName = browserName,
                SessionId = sessionId,
                CreatedDate = DateTime.Now
            });

            await _context.SaveChangesAsync();
        }

        public async Task RecordLogoutAsync(int userId)
        {
            var log = await _context.UserLoginLogs
                .Where(x => x.UserId == userId && x.LogoutTime == null)
                .OrderByDescending(x => x.LoginTime)
                .FirstOrDefaultAsync();

            if (log != null)
            {
                log.LogoutTime = DateTime.Now;
            }

            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.IsOnline = false;
                user.LastSeenAt = DateTime.Now;
            }

            await _context.SaveChangesAsync();
        }
    }
}
