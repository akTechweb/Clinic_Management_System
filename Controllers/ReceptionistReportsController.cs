using InfinityCoderzzz_CMSV2026.Services;
using Microsoft.AspNetCore.Mvc;

namespace InfinityCoderzzz_CMSV2026.Controllers
{
    public class ReceptionistReportsController : Controller
    {
        private readonly IPatientService _patientService;
        private readonly IAppointmentService _appointmentService;
        private readonly IBillService _billService;

        public ReceptionistReportsController(
            IPatientService patientService,
            IAppointmentService appointmentService,
            IBillService billService)
        {
            _patientService = patientService;
            _appointmentService = appointmentService;
            _billService = billService;
        }

        public IActionResult Index(
            string? reportType,
            DateTime? fromDate,
            DateTime? toDate,
            int? doctorId,
            int? departmentId)
        {
            reportType = string.IsNullOrWhiteSpace(reportType)
                ? "Appointment"
                : reportType;

            DateTime startDate = fromDate ?? DateTime.Today;
            DateTime endDate = toDate ?? DateTime.Today;

            var doctors = _appointmentService.GetAllActiveDoctors();

            var departments = doctors
                .Where(d => !string.IsNullOrWhiteSpace(d.DepartmentName))
                .GroupBy(d => new { d.DepartmentId, d.DepartmentName })
                .Select(g => g.Key)
                .ToList();

            var appointments = _appointmentService.GetAppointmentsByFilter(
                departmentId,
                doctorId,
                null,
                startDate,
                endDate);

            var patients = _patientService.GetAllPatients()
                .Where(p =>
                    p.RegistrationDate.HasValue &&
                    p.RegistrationDate.Value.Date >= startDate.Date &&
                    p.RegistrationDate.Value.Date <= endDate.Date)
                .ToList();

            var bills = _billService.GetAllBills()
                .Where(b =>
                    b.BillDate.HasValue &&
                    b.BillDate.Value.Date >= startDate.Date &&
                    b.BillDate.Value.Date <= endDate.Date)
                .ToList();

            ViewBag.ReportType = reportType;
            ViewBag.FromDate = startDate.ToString("yyyy-MM-dd");
            ViewBag.ToDate = endDate.ToString("yyyy-MM-dd");
            ViewBag.DoctorId = doctorId;
            ViewBag.DepartmentId = departmentId;

            ViewBag.Doctors = doctors;
            ViewBag.Departments = departments;

            ViewBag.AppointmentList = appointments;
            ViewBag.PatientList = patients;
            ViewBag.BillList = bills;

            ViewBag.TotalAppointments = appointments.Count;
            ViewBag.CompletedAppointments = appointments.Count(a => a.Status == "Completed");
            ViewBag.PendingAppointments = appointments.Count(a => a.Status == "Scheduled");
            ViewBag.CancelledAppointments = appointments.Count(a => a.Status == "Cancelled");

            ViewBag.TotalRegistrations = patients.Count;

            ViewBag.TotalCollection = bills
                .Where(b => b.Status == "Paid")
                .Sum(b => b.TotalAmount);

            ViewBag.PendingCollection = bills
                .Where(b => b.Status != "Paid")
                .Sum(b => b.TotalAmount);

            return View();
        }
    }
}