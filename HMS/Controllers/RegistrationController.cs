using HMS.Entities;
using HMS.Models;
using HMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using ZXing;
using ZXing.Common;

namespace HMS.Controllers
{
    public class RegistrationController : BaseController
    {
        private readonly IRegistrationService _service;
        private readonly IWebHostEnvironment _environment;
        public RegistrationController( IRegistrationService service, IWebHostEnvironment environment)
        {
            _service = service;
            _environment = environment;
        }

        [HttpGet]
        public async Task<IActionResult> Index(OPDSearchViewModel model)
        {
            model = await _service.SearchOPDAsync(model);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View(await _service.BuildCreateAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegistrationFormViewModel vm, IFormFile PatientImage)
        {
            if (!ModelState.IsValid)
            {
                vm = await _service.BuildFormAsync(vm.Patient);
                return View(vm);
            }

            if (PatientImage != null)
            {
                string folder = Path.Combine("wwwroot/uploads/patients");

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string fileName = Guid.NewGuid() + Path.GetExtension(PatientImage.FileName);
                string path = Path.Combine(folder, fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await PatientImage.CopyToAsync(stream);
                }

                vm.Patient.PatientPicture = fileName;
            }

            long patientId = await _service.CreateAsync(vm.Patient);

            Success("Patient Registered Successfully.");

            return RedirectToAction(nameof(PrintOPCard), new
            {
                id = patientId
            });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var vm = await _service.BuildEditAsync(id);
            if (vm == null) return NotFound();

            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RegistrationFormViewModel vm, IFormFile? PatientImage)
        {
            if (!ModelState.IsValid)
            {
                Error("Please correct the highlighted fields.");
                vm = await _service.BuildFormAsync(vm.Patient);
                return View(vm);
            }
            if (PatientImage != null)
            {
                string folder = Path.Combine(_environment.WebRootPath, "uploads", "patients");

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string fileName = Guid.NewGuid() + Path.GetExtension(PatientImage.FileName);

                using var stream = new FileStream(
                    Path.Combine(folder, fileName),
                    FileMode.Create);

                await PatientImage.CopyToAsync(stream);

                vm.Patient.PatientPicture = fileName;
            }

            await _service.UpdateAsync(vm.Patient);

            Success("Patient updated successfully.");
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(long id)
        {
            var data = await _service.GetByIdAsync(id);
            if (data == null) return NotFound();

            return View(data);
        }

        public async Task<IActionResult> Delete(long id)
        {
            var data = await _service.GetForDeleteAsync(id);
            if (data == null) return NotFound();

            return View(data);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (await _service.DeleteAsync(id))
                Success("Patient deleted successfully.");
            else
                Error("Record not found.");

            return RedirectToAction(nameof(Index));
        }

        public async Task<JsonResult> GetStatesByCountry(int countryId)
        {
            var data = await _service.GetStatesByCountryAsync(countryId);

            return Json(data.Select(x => new
            {
                stateId = x.StateId,
                stateName = x.StateName
            }));
        }

        public async Task<JsonResult> GetMandalsByState(int stateId)
        {
            var data = await _service.GetMandalsByStateAsync(stateId);

            return Json(data.Select(x => new
            {
                mandalId = x.MandalId,
                mandalName = x.MandalName
            }));
        }

        public async Task<JsonResult> GetCitiesByMandal(int mandalId)
        {
            var data = await _service.GetCitiesByMandalAsync(mandalId);

            return Json(data.Select(x => new
            {
                cityId = x.CityId,
                cityName = x.CityName
            }));
        }

        [HttpGet]       
        public async Task<IActionResult> PrintOPCard(long id)
        {
            var patient = await _service.GetByIdAsync(id);
            var hospital = await _service.GetHospitalAsync();

            ViewBag.Hospital = hospital;

            if (patient == null)
                return NotFound();

            string qrData = $@"
            UHID:{patient.UHID}
            OPNO:{patient.OpNo}
            NAME:{patient.PatientName}
            FATHER:{patient.FatherOrHusband}
            GENDER:{patient.Gender}
            DOB:{patient.Dob:dd-MM-yyyy}
            PHONE:{patient.Phone}
            AADHAR:{patient.AadharNo}
            ADDRESS:{patient.Address}
            CATEGORY:{patient.CategoryId}
            ";

            ViewBag.QRCode = GenerateQRCode(qrData);
            ViewBag.BarCode = GenerateBarcode(qrData);

            return View(patient);
        }

        private string GenerateQRCode(string text)
        {
            using QRCodeGenerator generator = new();

            QRCodeData data = generator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);

            Base64QRCode qrCode = new(data);

            return qrCode.GetGraphic(20);
        }

        private string GenerateBarcode(string text)
        {
            var writer = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.CODE_128,
                Options = new EncodingOptions
                {
                    Height = 1000,
                    Width = 320,
                    Margin = 2
                }
            };

            var pixelData = writer.Write(text);

            using Bitmap bitmap = new(pixelData.Width, pixelData.Height);

            var bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, pixelData.Width, pixelData.Height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppRgb);

            System.Runtime.InteropServices.Marshal.Copy(
                pixelData.Pixels,
                0,
                bitmapData.Scan0,
                pixelData.Pixels.Length);

            bitmap.UnlockBits(bitmapData);

            using MemoryStream ms = new();

            bitmap.Save(ms, ImageFormat.Png);

            return Convert.ToBase64String(ms.ToArray());
        }
    }
}