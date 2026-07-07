namespace HMS.Models
{
    public class EMRCasesheetDbVm
    {
        public int EMRId { get; set; }

        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public int StudentId { get; set; }

        public int? AllotId { get; set; }
        public int? ReferredId { get; set; }

        public string? OpNo { get; set; }

        public string? PatientName { get; set; }
        public string? Gender { get; set; }
        public int? Age { get; set; }
        public string? Phone { get; set; }

        public string? DoctorName { get; set; }
        public string? StudentName { get; set; }

        public DateTime CaseDate { get; set; }


        // Arrival Details
        public string? ArrivalMode { get; set; }
        public DateTime? ArrivalTime { get; set; }
        public string? BroughtBy { get; set; }


        // Triage
        public string? TriageCategory { get; set; }
        public DateTime? TriageTime { get; set; }
        public string? TriageNotes { get; set; }


        // History
        public string? ChiefComplaint { get; set; }

        public string? HistoryOfPresentIllness { get; set; }
        public string? PastMedicalHistory { get; set; }
        public string? PastSurgicalHistory { get; set; }
        public string? FamilyHistory { get; set; }
        public string? PersonalHistory { get; set; }

        public string? DrugHistory { get; set; }
        public string? AllergyHistory { get; set; }


        // Vitals
        public string? BloodPressure { get; set; }
        public int? PulseRate { get; set; }
        public decimal? Temperature { get; set; }
        public int? RespiratoryRate { get; set; }
        public int? SpO2 { get; set; }

        public decimal? Weight { get; set; }
        public decimal? Height { get; set; }
        public decimal? BMI { get; set; }

        public decimal? GRBS { get; set; }

        public int? PainScore { get; set; }
        public int? GCSScore { get; set; }

        public string? Pupils { get; set; }


        // Examination
        public string? GeneralExamination { get; set; }

        public string? CVSExamination { get; set; }

        public string? RSExamination { get; set; }

        public string? AbdomenExamination { get; set; }

        public string? CNSExamination { get; set; }

        public string? LocalExamination { get; set; }


        // Diagnosis / Treatment
        public string? ProvisionalDiagnosis { get; set; }

        public string? InvestigationsOrdered { get; set; }

        public string? InvestigationResults { get; set; }

        public string? ProceduresPerformed { get; set; }

        public string? EmergencyTreatment { get; set; }

        public string? MedicationsAdministered { get; set; }

        public string? IVFluids { get; set; }


        // Final Details
        public string? FinalDiagnosis { get; set; }

        public string? Disposition { get; set; }

        public string? AdmittedWard { get; set; }

        public string? ReferredHospital { get; set; }

        public string? DischargeAdvice { get; set; }

        public string? FollowUpAdvice { get; set; }


        // Approval
        public bool IsSentForApproval1 { get; set; }

        public bool Approval1Status { get; set; }

        public bool IsSentForApproval2 { get; set; }

        public bool Approval2Status { get; set; }
    }
}