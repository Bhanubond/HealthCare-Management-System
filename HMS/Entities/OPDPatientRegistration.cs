using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Entities
{
    [Table("OPDPatientRegistration")]
    public class OPDPatientRegistration
    {
        [Key]
        public int PatientId { get; set; }

        // Business Identifiers
        [StringLength(50)]
        public string? UHID { get; set; }

        [StringLength(50)]
        public string? OpNo { get; set; }

        public DateTime RegDate { get; set; } = DateTime.Now;

        // Patient Identity
        [StringLength(50)]
        public string? Title { get; set; }

        [Required, StringLength(200)]
        public string PatientName { get; set; }

        [StringLength(200)]
        public string? FatherOrHusband { get; set; }

        // Demographics
        public DateTime? Dob { get; set; }

        [StringLength(20)]
        public string? Gender { get; set; }

        [StringLength(50)]
        public string? MaritalStatus { get; set; }

        // Contact
        [StringLength(20)]
        public string? Phone { get; set; }

        // Address
        [StringLength(500)]
        public string? Address { get; set; }

        public int? CountryId { get; set; }
        public int? StateId { get; set; }
        public int? MandalId { get; set; }
        public int? CityId { get; set; }

        // Identification
        [StringLength(20)]
        public string? AadharNo { get; set; }

        // Billing
        public int? CategoryId { get; set; }
        public int? PaymodeId { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? DiscountPer { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? TotalAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? NetAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? PaidAmount { get; set; }

        // Documents
        [StringLength(255)]
        public string? PatientPicture { get; set; }

        [StringLength(500)]
        public string? PatientPictureFilePath { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        // Status
        public DateTime? OPCardValidityDate { get; set; }

        public bool IsRegCancelled { get; set; } = false;

        // Audit Fields
        public int? CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        [StringLength(100)]
        public string? CreatedSystem { get; set; }

        [StringLength(100)]
        public string? ModifiedSystem { get; set; }
    }
}