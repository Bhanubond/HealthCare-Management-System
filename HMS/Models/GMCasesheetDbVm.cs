namespace HMS.Models
{
    public class GMCasesheetDbVm
    {
        public int GMID { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public int StudentId { get; set; }
        public int? AllotId { get; set; }
        public int? ReferredId { get; set; }
        public string? OpNo { get; set; }

        public string? PatientName { get; set; }
        public string? Gender { get; set; }
        public int? Age { get; set; }

        public string? DoctorName { get; set; }
        public string? StudentName { get; set; }

        public string? ChiefComplaint { get; set; }
        public string? Symptoms { get; set; }
        public string? Diagnosis { get; set; }
        public string? Prescription { get; set; }
        public string? Notes { get; set; }

        public string HistoryOfPresentIllness { get; set; }
        public string PastMedicalHistory { get; set; }
        public string PastSurgicalHistory { get; set; }
        public string FamilyHistory { get; set; }
        public string PersonalHistory { get; set; }

        public string BloodPressure { get; set; }
        public int? PulseRate { get; set; }
        public decimal? Temperature { get; set; }
        public int? RespiratoryRate { get; set; }
        public int? SpO2 { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Height { get; set; }
        public decimal? BMI { get; set; }

        public string GeneralExamination { get; set; }
        public string CVSExamination { get; set; }
        public string RSExamination { get; set; }
        public string AbdomenExamination { get; set; }
        public string CNSExamination { get; set; }

        public bool IsSentForApproval1 { get; set; } = false;
        public bool Approval1Status { get; set; } = false;
        public bool IsSentForApproval2 { get; set; } = false;
        public bool Approval2Status { get; set; } = false;
    }
}
