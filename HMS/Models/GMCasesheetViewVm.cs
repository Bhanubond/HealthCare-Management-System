using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Models
{
    public class GMCasesheetViewVm
    {
        //[NotMapped]
        public int PatientId { get; set; }
        public string? OpNo { get; set; }
        public string? PatientName { get; set; }
        public int? Age { get; set; }
        public string? Gender { get; set; }

        [NotMapped]
        public string? DoctorName { get; set; }

        [NotMapped]
        public string? StudentName { get; set; }

        [NotMapped]
        public int DoctorId { get; set; }

        [NotMapped]
        public int StudentId { get; set; }

        [NotMapped]
        public int? AllotId { get; set; }

        [NotMapped]
        public int? ReferredId { get; set; }
    }
}
