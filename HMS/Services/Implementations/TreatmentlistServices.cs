using HMS.Data;
using HMS.Entities;
using HMS.Services.Interfaces;
using HMS.Models;
using Microsoft.EntityFrameworkCore;

namespace HMS.Services.Implementations
{
    public class TreatmentlistServices : ITreatmentlistServices
    {
        private readonly HmsDbContext _context;

        public TreatmentlistServices(HmsDbContext context)
        {
            _context = context;
        }

        public async Task<List<TreatmentServiceListVM>> GetAllAsync()
        {
            return await _context.TreatmentServices
                .OrderBy(x => x.ServiceName)
                .Select(x => new TreatmentServiceListVM
                {
                    ServiceID = x.ServiceID,
                    ServiceCode = x.ServiceCode,
                    ServiceName = x.ServiceName,
                    DepartmentName = x.Department.DeptName,
                    Cost = x.Cost,
                    IsActive = x.IsActive
                })
                .ToListAsync();
        }

        public async Task<TreatmentServices?> GetByIdAsync(int id)
        {
            return await _context.TreatmentServices
                .Include(x => x.Department)
                .FirstOrDefaultAsync(x => x.ServiceID == id);
        }

        public async Task AddAsync(TreatmentServices model)
        {
            model.CreatedAt = DateTime.Now;
            model.UpdatedAt = DateTime.Now;

            await _context.TreatmentServices.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TreatmentServices model)
        {
            var data = await _context.TreatmentServices
                .FirstOrDefaultAsync(x => x.ServiceID == model.ServiceID);

            if (data != null)
            {
                data.ServiceCode = model.ServiceCode;
                data.ServiceName = model.ServiceName;
                data.DeptId = model.DeptId;
                data.Description = model.Description;
                data.Cost = model.Cost;
                data.IsActive = model.IsActive;
                data.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var data = await _context.TreatmentServices.FindAsync(id);

            if (data != null)
            {
                _context.TreatmentServices.Remove(data);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<MASDepartment>> GetDepartmentsAsync()
        {
            return await _context.MASDepartments
                .Where(x => x.IsActive)
                .OrderBy(x => x.DeptName)
                .ToListAsync();
        }
    }
}