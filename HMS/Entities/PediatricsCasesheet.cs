using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Entities
{
    [Table("PediatricsCasesheet")]
    public class PediatricsCasesheet : EntityBase
    {
        [Key]
        public int PedoID { get; set; }
        public int PatientId { get; set; }

        public int DoctorId { get; set; }

        public int StudentId { get; set; }

        public int? AllotId { get; set; }

        public int? ReferredId { get; set; }

        public string? OpNo { get; set; }

        public DateTime CaseDate { get; set; }

        public string? ChiefComplaint { get; set; }

        public string? HistoryOfPresentIllness { get; set; }

        public string? PastMedicalHistory { get; set; }

        public string? FamilyHistory { get; set; }

        public string? BirthHistory { get; set; }

        public decimal? BirthWeight { get; set; }

        public string? ImmunizationStatus { get; set; }

        public decimal? Weight { get; set; }

        public decimal? Height { get; set; }

        public string? GrowthStatus { get; set; }

        public decimal? Temperature { get; set; }

        public int? PulseRate { get; set; }

        public int? RespiratoryRate { get; set; }

        public int? SpO2 { get; set; }

        public string? GeneralExamination { get; set; }

        public string? SystemicExamination { get; set; }

        public string? Diagnosis { get; set; }

        public string? Prescription { get; set; }

        public string? Advice { get; set; }

        public string? Notes { get; set; }

        public bool IsSentForApproval1 { get; set; } = false;

        public bool Approval1Status { get; set; } = false;

        public bool IsSentForApproval2 { get; set; } = false;

        public bool Approval2Status { get; set; } = false;

        public DateTime CreatedDate { get; set; }

        public int? CreatedBy { get; set; }

        public string? CreatedSystem { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public int? ModifiedBy { get; set; }

        public string? ModifiedSystem { get; set; }
    }
}