using HMS.Data;
using HMS.Entities;
using HMS.Models;
using HMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HMS.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly HmsDbContext _context;

        public UserService(HmsDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserIndexViewModel>> GetAllAsync()
        {
            return await (
                from u in _context.Users
                where u.UserId != 1
                join s in _context.Students on u.StudentId equals s.StudentId into sj
                from s in sj.DefaultIfEmpty()
                join d in _context.Doctors on u.DoctorId equals d.DoctorId into dj
                from d in dj.DefaultIfEmpty()
                join sd in _context.MASDepartments on s.DepartmentId equals sd.DeptId into sdj
                from sd in sdj.DefaultIfEmpty()
                join dd in _context.MASDepartments on d.DepartmentId equals dd.DeptId into ddj
                from dd in ddj.DefaultIfEmpty()
                select new UserIndexViewModel
                {
                    UserId = u.UserId,
                    UserName = u.UserName,
                    Name = s != null ? s.StudentName : d != null ? d.DoctorName : null,
                    DeptName = s != null ? sd != null ? sd.DeptName : null : d != null ? dd != null ? dd.DeptName : null : null,
                    Active = u.Active
                }
            )
            .OrderByDescending(x => x.UserId)
            .ToListAsync();
        }

        public async Task<UserDetailsDto?> GetDetailsAsync(int id)
        {
            return await BuildDetailsQuery()
                .FirstOrDefaultAsync(x => x.UserId == id);
        }

        public async Task<UserDetailsDto?> GetForDeleteAsync(int id)
        {
            return await GetDetailsAsync(id);
        }

        public async Task<UserFormViewModel> BuildCreateAsync()
        {
            return await BuildFormAsync(new UserFormViewModel
            {
                Active = true,
                EnableViewAllPatient = false,
                PasswordReset = false,
                EnableLiveChat = false,
                IsOnline = false,
                CreatedDate = DateTime.Now
            });
        }

        public async Task<UserFormViewModel?> BuildEditAsync(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserId == id);
            if (user == null)
            {
                return null;
            }

            return await BuildFormAsync(MapToForm(user));
        }

        public async Task<UserFormViewModel> BuildFormAsync(UserFormViewModel model)
        {
            model.StudentOptions = await BuildStudentOptionsAsync(model.StudentId);
            model.DoctorOptions = await BuildDoctorOptionsAsync(model.DoctorId);
            return model;
        }

        public async Task<bool> CreateAsync(UserFormViewModel model)
        {
            var user = model.ToEntity();
            user.CreatedDate ??= DateTime.Now;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(UserFormViewModel model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserId == model.UserId);
            if (user == null)
            {
                return false;
            }

            user.UserName = model.UserName;
            user.Password = model.Password;
            user.StudentId = model.StudentId;
            user.DoctorId = model.DoctorId;
            user.CategoryId = model.CategoryId;
            user.Active = model.Active;
            user.EnableViewAllPatient = model.EnableViewAllPatient;
            user.PasswordReset = model.PasswordReset;
            user.EnableLiveChat = model.EnableLiveChat;
            user.IsOnline = model.IsOnline;
            user.CreatedBy = model.CreatedBy;
            user.CreatedDate = model.CreatedDate;
            user.ModifiedBy = model.ModifiedBy;
            user.ModifiedDate = DateTime.Now;
            user.LastSeenAt = model.LastSeenAt;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return false;
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<IEnumerable<SelectListItem>> BuildStudentOptionsAsync(long? selectedValue)
        {
            var items = await _context.Students
                .Where(x => x.IsActive)
                .OrderBy(x => x.StudentName)
                .ToListAsync();

            return new SelectList(items, "StudentId", "StudentName", selectedValue);
        }

        private async Task<IEnumerable<SelectListItem>> BuildDoctorOptionsAsync(long? selectedValue)
        {
            var items = await _context.Doctors
                .Where(x => x.IsActive)
                .OrderBy(x => x.DoctorName)
                .ToListAsync();

            return new SelectList(items, "DoctorId", "DoctorName", selectedValue);
        }

        private UserFormViewModel MapToForm(Users user)
        {
            return new UserFormViewModel
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Password = user.Password,
                StudentId = user.StudentId,
                DoctorId = user.DoctorId,
                CategoryId = user.CategoryId,
                Active = user.Active,
                EnableViewAllPatient = user.EnableViewAllPatient,
                PasswordReset = user.PasswordReset,
                EnableLiveChat = user.EnableLiveChat,
                IsOnline = user.IsOnline,
                CreatedBy = user.CreatedBy,
                CreatedDate = user.CreatedDate,
                ModifiedBy = user.ModifiedBy,
                ModifiedDate = user.ModifiedDate,
                LastSeenAt = user.LastSeenAt
            };
        }

        private IQueryable<UserDetailsDto> BuildDetailsQuery()
        {
            return
                from u in _context.Users
                join s in _context.Students on u.StudentId equals s.StudentId into sj
                from s in sj.DefaultIfEmpty()
                join d in _context.Doctors on u.DoctorId equals d.DoctorId into dj
                from d in dj.DefaultIfEmpty()
                join sd in _context.MASDepartments on s.DepartmentId equals sd.DeptId into sdj
                from sd in sdj.DefaultIfEmpty()
                join dd in _context.MASDepartments on d.DepartmentId equals dd.DeptId into ddj
                from dd in ddj.DefaultIfEmpty()
                select new UserDetailsDto
                {
                    UserId = u.UserId,
                    UserName = u.UserName,
                    Name = s != null ? s.StudentName : d != null ? d.DoctorName : null,
                    DeptName = s != null ? sd != null ? sd.DeptName : null : d != null ? dd != null ? dd.DeptName : null : null,
                    CategoryId = u.CategoryId,
                    Active = u.Active,
                    EnableViewAllPatient = u.EnableViewAllPatient,
                    PasswordReset = u.PasswordReset,
                    EnableLiveChat = u.EnableLiveChat,
                    IsOnline = u.IsOnline,
                    CreatedBy = u.CreatedBy,
                    CreatedDate = u.CreatedDate,
                    ModifiedBy = u.ModifiedBy,
                    ModifiedDate = u.ModifiedDate,
                    LastSeenAt = u.LastSeenAt
                };
        }
    }
}
