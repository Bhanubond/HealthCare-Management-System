using HMS.Entities;

namespace HMS.Models
{
    public class MedicationPartialVm
    {
        public List<PatientMedicationVm> Medications { get; set; }
        public List<MASMedication> MedicationMaster { get; set; }
    }
}
