using HMS.Data;
using HMS.Models;
using HMS.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HMS.Services.Implementations
{
    public class DashboardService : IDashboardService
    {
        private readonly HmsDbContext _context;

        public DashboardService(HmsDbContext context)
        {
            _context = context;
        }

        public async Task<List<DashboardDepartmentViewModel>> GetDepartmentsAsync()
        {
            return await _context.MASDepartments
                .AsNoTracking()
                .Where(x => x.IsActive && !x.DelInd)
                .OrderBy(x => x.DisplayOrder ?? int.MaxValue)
                .ThenBy(x => x.DeptName)
                .Select(x => new DashboardDepartmentViewModel
                {
                    DeptId = x.DeptId,
                    DeptName = x.DeptName,
                    TreeDeptName = x.TreeDeptName,
                    CssClass = x.CssClass,
                    ScreenCount = _context.MASDeptScreenMappings.Count(m =>
                        m.IsActive &&
                        m.DeptId == x.DeptId)
                })
                .ToListAsync();
        }

        public async Task<List<DashboardScreenCardViewModel>> GetScreensAsync(int departmentId)
        {
            return await (
                    from map in _context.MASDeptScreenMappings.AsNoTracking()
                    join screen in _context.MASScreens.AsNoTracking()
                        on map.ScreenId equals screen.ScreenId
                    where map.DeptId == departmentId
                          && map.IsActive
                          && screen.IsActive
                          && !screen.DelInd
                    orderby screen.OrderDisplay ?? int.MaxValue, screen.ScreenDisplayName
                    select new DashboardScreenCardViewModel
                    {
                        ScreenId = screen.ScreenId,
                        ScreenName = screen.ScreenName,
                        ScreenDisplayName = screen.ScreenDisplayName,
                        Title = screen.Title,
                        ControllerName = screen.ControllerName,
                        ActionName = screen.ActionName
                    })
                .ToListAsync();
        }
    }
}
