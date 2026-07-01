namespace HMS.Models
{
    public class PatientMedicationVm
    {
        public int MedicationId { get; set; }
        public string? MedicationName { get; set; }
        public string? Frequency { get; set; }
        public string? Remarks { get; set; }
        public string? Duration { get; set; }
    }
}
