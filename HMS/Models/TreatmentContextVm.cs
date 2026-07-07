namespace HMS.Models
{
    public class TreatmentContextVm
    {
        public int? AllotId { get; set; }
        public int? ReferredId { get; set; }
        public int? DeptId { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
    }
}
