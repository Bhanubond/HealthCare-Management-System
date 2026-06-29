using HMS.Entities;

namespace HMS.Models
{
    public class AllotmentViewModel : EntityBase
    {
        public int PatientId { get; set; }
        public string OpNo { get; set; }
        public string PatientName { get; set; }
        public string CategoryName { get; set; }
        public string FromDepartment { get; set; }
        public int FromDeptId { get; set; }
        public int ToDeptId { get; set; }
        public string AllotmentStatus { get; set; }
        public DateTime? FromDate { get; set; }
    }
}
