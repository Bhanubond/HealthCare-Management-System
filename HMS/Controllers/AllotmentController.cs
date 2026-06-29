using HMS.Models;
using HMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

public class AllotmentController : Controller
{
    private readonly IAllotmentService _service;

    public AllotmentController(IAllotmentService service)
    {
        _service = service;
    }

    // Main Page (ALL departments)
    public async Task<IActionResult> Index(int deptId)
    {
        var patients = await _service.GetPatientsByDepartment(deptId);

        ViewBag.DeptId = deptId;
        return View(patients);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllotmentData(int patientId, int deptId)
    {
        var model = await _service.GetAllotFormData(patientId, deptId);

        return Json(new
        {
            patientId = model.PatientId,
            deptId = model.DeptId,
            patientName = model.PatientName,
            referredId = model.ReferredId,
            students = model.Students.Select(x => new
            {
                studentId = x.StudentId,
                studentName = x.StudentName
            }),
            doctors = model.Doctors.Select(x => new
            {
                doctorId = x.DoctorId,
                doctorName = x.DoctorName
            })
        });
    }

    // Save (COMMON)
    [HttpPost]
    public async Task<IActionResult> SaveAllotment(StudentAllotmentViewModel model)
    {
        await _service.SaveAllotment(model);
        return RedirectToAction("Index", new { deptId = model.DeptId });
    }
}