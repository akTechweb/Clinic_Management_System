using InfinityCoderzz_CMSV2026.Models;
using InfinityCoderzzz_CMSV2026.Services;
using Microsoft.AspNetCore.Mvc;

namespace InfinityCoderzz_CMSV2026.Controllers
{
    public class PatientsController : Controller
    {
        private readonly IPatientService _patientService;

        public PatientsController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Search");
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.GeneratedMMR = _patientService.GetNextPatientCode();
            return View(new Patient());
        }

        [HttpPost]
        public IActionResult Create(Patient patient)
        {
            try
            {
                ModelState.Remove("PatientCode");
                ModelState.Remove("BloodGroup");

                if (string.IsNullOrWhiteSpace(patient.BloodGroup))
                {
                    patient.BloodGroup = "Unknown";
                }

                ValidatePatient(patient);

                if (!ModelState.IsValid)
                {
                    TempData["ErrorMessage"] =
                        string.Join(" | ",
                        ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));

                    ViewBag.GeneratedMMR = _patientService.GetNextPatientCode();
                    return View(patient);
                }

                _patientService.RegisterPatient(patient);

                return RedirectToAction("RegisterSuccess", new
                {
                    patientId = patient.PatientId,
                    patientCode = patient.PatientCode,
                    fullName = patient.FullName,
                    mobile = patient.MobileNumber,
                    dob = patient.DOB?.ToString("dd-MM-yyyy") ?? "",
                    age = patient.Age?.ToString() ?? "",
                    gender = patient.Gender
                });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Registration failed: " + ex.Message;
                ViewBag.GeneratedMMR = _patientService.GetNextPatientCode();
                return View(patient);
            }
        }

        public IActionResult RegisterSuccess(
            int patientId,
            string patientCode,
            string fullName,
            string mobile,
            string dob,
            string age,
            string gender)
        {
            ViewBag.PatientId = patientId;
            ViewBag.PatientCode = patientCode;
            ViewBag.FullName = fullName;
            ViewBag.Mobile = mobile;
            ViewBag.DOB = dob;
            ViewBag.Age = age;
            ViewBag.Gender = gender;

            return View();
        }

        [HttpGet]
        public IActionResult Search(string? searchBy, string? searchText)
        {
            List<Patient> results = new();

            searchBy = string.IsNullOrWhiteSpace(searchBy) ? "MMR" : searchBy;
            searchText = searchText?.Trim();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                results = _patientService.SearchPatients(searchText);
            }

            ViewBag.SearchBy = searchBy;
            ViewBag.SearchText = searchText;
            ViewBag.PatientNotFound =
                !string.IsNullOrWhiteSpace(searchText) && !results.Any();

            return View(results);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            Patient? patient = _patientService.GetPatientById(id);

            if (patient == null)
                return NotFound();

            return View(patient);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Patient? patient = _patientService.GetPatientById(id);

            if (patient == null)
                return NotFound();

            return View(patient);
        }

        [HttpPost]
        public IActionResult Edit(Patient patient)
        {
            // Keep edit simple so save button definitely posts and updates.
            _patientService.UpdatePatient(patient);

            TempData["SuccessMessage"] = "Patient details edited successfully.";

            return RedirectToAction("Details", new { id = patient.PatientId });
        }

        private void ValidatePatient(Patient patient)
        {
            ModelState.Remove("PatientCode");
            ModelState.Remove("RegistrationDate");
            ModelState.Remove("IsActive");

            if (string.IsNullOrWhiteSpace(patient.BloodGroup))
            {
                patient.BloodGroup = "Unknown";
                ModelState.Remove("BloodGroup");
            }

            if (patient.DOB.HasValue)
            {
                DateTime today = DateTime.Today;

                if (patient.DOB.Value.Date > today)
                {
                    ModelState.AddModelError("DOB", "Date of birth cannot be a future date.");
                }
                else
                {
                    int age = today.Year - patient.DOB.Value.Year;

                    if (patient.DOB.Value.Date > today.AddYears(-age))
                        age--;

                    if (age < 0 || age > 100)
                    {
                        ModelState.AddModelError("DOB", "Age must be between 0 and 100 years.");
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(patient.MobileNumber) &&
                !System.Text.RegularExpressions.Regex.IsMatch(patient.MobileNumber, @"^[6-9][0-9]{9}$"))
            {
                ModelState.AddModelError("MobileNumber", "Mobile number must be 10 digits and start with 6, 7, 8, or 9.");
            }

            if (!string.IsNullOrWhiteSpace(patient.EmergencyContactNumber) &&
                !System.Text.RegularExpressions.Regex.IsMatch(patient.EmergencyContactNumber, @"^[6-9][0-9]{9}$"))
            {
                ModelState.AddModelError("EmergencyContactNumber", "Emergency contact must be 10 digits and start with 6, 7, 8, or 9.");
            }
        }

    }
}