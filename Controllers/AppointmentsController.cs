using InfinityCoderzz_CMSV2026.Models;
using InfinityCoderzzz_CMSV2026.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InfinityCoderzzz_CMSV2026.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IPatientService _patientService;

        public AppointmentsController(
            IAppointmentService appointmentService,
            IPatientService patientService)
        {
            _appointmentService = appointmentService;
            _patientService = patientService;
        }

        public IActionResult Index(
            int? departmentId,
            int? doctorId,
            string? patientCode,
            DateTime? fromDate,
            DateTime? toDate)
        {
            bool hasFilter =
                departmentId.HasValue ||
                doctorId.HasValue ||
                !string.IsNullOrWhiteSpace(patientCode) ||
                fromDate.HasValue ||
                toDate.HasValue;

            List<Appointment> appointments = hasFilter
                ? _appointmentService.GetAppointmentsByFilter(
                    departmentId,
                    doctorId,
                    patientCode,
                    fromDate,
                    toDate)
                : _appointmentService.GetAllAppointments();

            var doctors = _appointmentService.GetAllActiveDoctors();

            var departments = doctors
                .Where(d => !string.IsNullOrWhiteSpace(d.DepartmentName))
                .GroupBy(d => new { d.DepartmentId, d.DepartmentName })
                .Select(g => g.Key)
                .ToList();

            ViewBag.Doctors = new SelectList(
                doctors,
                "DoctorId",
                "FullName",
                doctorId);

            ViewBag.Departments = new SelectList(
                departments,
                "DepartmentId",
                "DepartmentName",
                departmentId);

            ViewBag.DepartmentId = departmentId;
            ViewBag.DoctorId = doctorId;
            ViewBag.PatientCode = patientCode;
            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");

            return View(appointments);
        }

        [HttpGet]
        public IActionResult Create(int? patientId)
        {
            LoadAppointmentData(patientId);

            var appointment = new Appointment
            {
                AppointmentDate = DateTime.Today
            };

            if (patientId.HasValue)
            {
                appointment.PatientId = patientId.Value;
            }

            return View(appointment);
        }

        [HttpPost]
        public IActionResult Create(Appointment appointment)
        {
            _appointmentService.BookAppointment(appointment, out string message);

            if (!string.IsNullOrWhiteSpace(message) &&
                message.ToUpper().Contains("SUCCESS"))
            {
                Appointment? savedAppointment =
                    _appointmentService.GetAppointmentById(appointment.AppointmentId);

                if (savedAppointment == null)
                {
                    TempData["ErrorMessage"] = "Appointment was created, but summary details could not be loaded.";
                    return RedirectToAction("Index");
                }

                return RedirectToAction("AppointmentSuccess", new
                {
                    appointmentId = savedAppointment.AppointmentId,
                    appointmentNumber = savedAppointment.AppointmentNumber,
                    tokenNumber = savedAppointment.TokenNumber,
                    patientName = savedAppointment.PatientName,
                    patientCode = savedAppointment.PatientCode,
                    doctorName = savedAppointment.DoctorName,
                    departmentName = savedAppointment.DepartmentName,
                    appointmentDate = savedAppointment.AppointmentDate.ToString("dd-MMM-yyyy"),
                    appointmentTime = savedAppointment.AppointmentTime.ToString(@"hh\:mm"),
                    patientId = savedAppointment.PatientId
                });
            }

            TempData["ErrorMessage"] = message;
            LoadAppointmentData(appointment.PatientId);

            return View(appointment);
        }

        public IActionResult AppointmentSuccess(
            int appointmentId,
            string appointmentNumber,
            int tokenNumber,
            string patientName,
            string patientCode,
            string doctorName,
            string departmentName,
            string appointmentDate,
            string appointmentTime,
            int patientId)
        {
            ViewBag.AppointmentId = appointmentId;
            ViewBag.AppointmentNumber = appointmentNumber;
            ViewBag.TokenNumber = tokenNumber;
            ViewBag.PatientName = patientName;
            ViewBag.PatientCode = patientCode;
            ViewBag.DoctorName = doctorName;
            ViewBag.DepartmentName = departmentName;
            ViewBag.AppointmentDate = appointmentDate;
            ViewBag.AppointmentTime = appointmentTime;
            ViewBag.PatientId = patientId;

            return View();
        }

        [HttpGet]
        public IActionResult GetBookedSlots(int doctorId, DateTime appointmentDate)
        {
            var appointments = _appointmentService.GetAppointmentsByFilter(
                null,
                doctorId,
                null,
                appointmentDate,
                appointmentDate);

            var bookedSlots = appointments
                .Where(a => a.Status != "Cancelled")
                .Select(a => a.AppointmentTime.ToString(@"hh\:mm\:ss"))
                .ToList();

            return Json(bookedSlots);
        }

        public IActionResult Details(int id)
        {
            Appointment? appointment = _appointmentService.GetAppointmentById(id);

            if (appointment == null)
                return NotFound();

            return View(appointment);
        }

        [HttpPost]
        public IActionResult Cancel(int id)
        {
            _appointmentService.CancelAppointment(id, out string message);

            if (!string.IsNullOrWhiteSpace(message) &&
                message.StartsWith("SUCCESS"))
            {
                TempData["SuccessMessage"] = "Appointment cancelled.";
            }
            else
            {
                TempData["ErrorMessage"] = message;
            }

            return RedirectToAction("Details", new { id });
        }

        private void LoadAppointmentData(int? selectedPatientId)
        {
            ViewBag.DoctorList = _appointmentService.GetAllActiveDoctors();

            ViewBag.Patients = new SelectList(
                _patientService.GetAllPatients(),
                "PatientId",
                "FullName",
                selectedPatientId);

            if (selectedPatientId.HasValue)
            {
                ViewBag.SelectedPatient =
                    _patientService.GetPatientById(selectedPatientId.Value);
            }
        }
    }
}