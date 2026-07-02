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
    public IActionResult Index(int deptId)
    {
        deptId = deptId == 0
            ? HttpContext.Session.GetInt32("DeptId") ?? 0
            : deptId;

        if (deptId == 0)
            return BadRequest("Department not selected");

        ViewBag.DeptId = deptId;
        ViewBag.FromDate = DateTime.Today;
        ViewBag.ToDate = DateTime.Today;

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetPatients(int deptId, DateTime fromDate, DateTime toDate)
    {
        var data = await _service.GetPatientsByDepartment(deptId, fromDate, toDate);
        return Json(data);
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

    [HttpPost]
    public async Task<IActionResult> SaveAllotment(StudentAllotmentViewModel model)
    {
        await _service.SaveAllotment(model);
        return RedirectToAction("Index", new { deptId = model.DeptId });
    }
}