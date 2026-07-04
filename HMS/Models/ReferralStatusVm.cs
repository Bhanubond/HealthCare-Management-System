using HMS.Common;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HMS.Models
{
    public class ReferralStatusVm
    {
        public int FromDeptId { get; set; }

        public List<SelectListItem> Departments { get; set; } = new();

        public List<int> SelectedToDeptIds { get; set; } = new();

        public Dictionary<int, string> Reasons { get; set; } = new();
        public List<ReferralDisplayVm> ExistingReferrals { get; set; } = new();
        public int PatientId { get;  set; }
    }
}
