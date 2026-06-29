using HMS.Models;

namespace HMS.Services.Interfaces
{
    public interface IPatientService
    {
        Task<PatientDetailsViewModel> GetPatientDetails(int patientId);
    }
}
