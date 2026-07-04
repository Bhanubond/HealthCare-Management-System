namespace HMS.Models
{
    public class GMApprovalQueueVm
    {
        public int GMID { get; set; }
        public string OpNo { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public string StudentName { get; set; }
        public string DepartmentName { get; set; }
        public string CategoryName { get; set; }
        public DateTime AllotDate { get; set; }

        public bool? IsSentForApproval1 { get; set; }
        public bool? Approval1Status { get; set; }

        public bool? IsSentForApproval2 { get; set; }
        public bool? Approval2Status { get; set; }
    }
}
