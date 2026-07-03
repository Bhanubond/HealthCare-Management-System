using HMS.Entities;
using HMS.Models;

namespace HMS.Services.Interfaces
{
    public interface IMedicationService
    {
        Task<List<MASMedication>> GetActiveMedications();

        Task<List<PatientMedicationVm>> GetPatientMedications(int patientId);

        Task SavePatientMedications(
            int patientId,
            IEnumerable<PatientMedicationVm>? medications);
    }
}