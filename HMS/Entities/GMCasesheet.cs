using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Entities
{
    [Table("GMCasesheet")]
    public class GMCasesheet : EntityBase
    {
        [Key]
        public int GMID { get; set; }

        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public int StudentId { get; set; }
        public int? AllotId { get; set; }
        public int? ReferredId { get; set; }

        public string? OpNo { get; set; }
        public DateTime CaseDate { get; set; }

        // =========================
        // 🧍 HISTORY SECTION
        // =========================
        public string? HistoryOfPresentIllness { get; set; }
        public string? PastMedicalHistory { get; set; }
        public string? PastSurgicalHistory { get; set; }
        public string? FamilyHistory { get; set; }
        public string? PersonalHistory { get; set; } // smoking, alcohol, diet

        // =========================
        // 📊 VITALS
        // =========================
        public string? BloodPressure { get; set; }
        public int? PulseRate { get; set; }
        public decimal? Temperature { get; set; }
        public int? RespiratoryRate { get; set; }
        public int? SpO2 { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Height { get; set; }
        public decimal? BMI { get; set; }

        // =========================
        // 🩻 CLINICAL EXAMINATION
        // =========================
        public string? GeneralExamination { get; set; }
        public string? CVSExamination { get; set; }
        public string? RSExamination { get; set; }
        public string? AbdomenExamination { get; set; }
        public string? CNSExamination { get; set; }

        // =========================
        // 🧾 EXISTING CORE FIELDS
        // =========================
        public string? ChiefComplaint { get; set; }
        public string? Symptoms { get; set; }
        public string? Diagnosis { get; set; }
        public string? Prescription { get; set; }
        public string? Notes { get; set; }

        // =========================
        // 🔐 APPROVAL FLOW
        // =========================
        public bool IsSentForApproval1 { get; set; } = false;
        public bool Approval1Status { get; set; } = false;
        public bool IsSentForApproval2 { get; set; } = false;
        public bool Approval2Status { get; set; } = false;

        // =========================
        // 🧾 AUDIT FIELDS
        // =========================
        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public string? CreatedSystem { get; set; }

        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public string? ModifiedSystem { get; set; }
    }
}