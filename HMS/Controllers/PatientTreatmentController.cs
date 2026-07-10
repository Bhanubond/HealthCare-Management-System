using HMS.Models;
using HMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HMS.Controllers
{
    public class PatientTreatmentController : Controller
    {
        private readonly IPatientTreatmentService _service;


        public PatientTreatmentController(
            IPatientTreatmentService service)
        {
            _service = service;
        }



        [HttpGet]
        public async Task<IActionResult> Add(int caseSheetId,int patientId, int deptId, int doctorId)
        {
            var services = await _service.GetDepartmentServices(deptId);

            var model = new PatientTreatmentVM
            {
                CaseSheetId = caseSheetId,
                PatientId = patientId,
                DeptId = deptId,
                DoctorId = doctorId,

                Services = services.Select(x => new PatientServiceVM
                {
                    ServiceID = x.ServiceID,
                    ServiceName = x.ServiceName,
                    Rate = x.Cost
                }).ToList()
            };

            return PartialView("_AddTreatment", model);
        }



        [HttpPost]
        public async Task<IActionResult> Add(
            PatientTreatmentVM model)
        {

            await _service.SavePatientTreatments(model);


            return Json(new
            {
                success = true
            });

        }

    }
}