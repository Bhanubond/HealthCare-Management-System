using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Entities
{
    [Table("PatientMedicationDetails")]
    public class PatientMedicationDetails
    {
        [Key]
        public int PatientMedicationId { get; set; }

        public int PatientId { get; set; }
        public int MedicationId { get; set; }
        public string? Frequency { get; set; }
        public string? Remarks { get; set; }
        public string? Duration { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int? CreatedBy { get; set; }
    }
}
