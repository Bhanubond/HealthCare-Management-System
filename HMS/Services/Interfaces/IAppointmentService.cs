using HMS.Entities;
using HMS.Models;

namespace HMS.Services.Interfaces
{
    public interface IAppointmentService
    {
        Task SaveFollowUpAsync(FollowUpVm model);
        Task UpdateFollowUpAsync(FollowUpVm model);
        Task<FollowUpVm?> GetFollowUpAsync(int followUpId);
    }
}
