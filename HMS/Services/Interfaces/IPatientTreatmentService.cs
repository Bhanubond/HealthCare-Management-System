using HMS.Entities;
using HMS.Entities.BillingDetails;
using HMS.Models;

namespace HMS.Services.Interfaces
{
    public interface IPatientTreatmentService
    {

        Task<List<TreatmentServices>> GetDepartmentServices(int deptId);


        Task SavePatientTreatments(PatientTreatmentVM model);


        Task<List<PatientTreatment>> GetPatientTreatments(
            int caseSheetId
        );

    }
}
