using InfinityCoderzzz_CMSV2026.Services;
using Microsoft.AspNetCore.Mvc;

namespace InfinityCoderzzz_CMSV2026.Controllers
{
    public class ReceptionistsController : Controller
    {
        private readonly IPatientService _patientService;
        private readonly IAppointmentService _appointmentService;
        private readonly IBillService _billService;

        public ReceptionistsController(
            IPatientService patientService,
            IAppointmentService appointmentService,
            IBillService billService)
        {
            _patientService = patientService;
            _appointmentService = appointmentService;
            _billService = billService;
        }

        public IActionResult Dashboard()
        {
            ViewBag.UserName = HttpContext.Session.GetString("FullName") ?? "Receptionist";

            var patients = _patientService.GetAllPatients();

            var todayAppointments = _appointmentService.GetAppointmentsByFilter(
                null, null, null, DateTime.Today, DateTime.Today);

            var tomorrowAppointments = _appointmentService.GetAppointmentsByFilter(
                null, null, null, DateTime.Today.AddDays(1), DateTime.Today.AddDays(1));

            var todayBills = _billService.GetAllBills()
                .Where(b => b.BillDate.HasValue &&
                            b.BillDate.Value.Date == DateTime.Today &&
                            b.Status == "Paid")
                .ToList();

            ViewBag.TotalPatients = patients.Count;
            ViewBag.TodayAppointments = todayAppointments.Count;
            ViewBag.TomorrowAppointments = tomorrowAppointments.Count;
            ViewBag.TodayCollection = todayBills.Sum(b => b.TotalAmount);

            ViewBag.RecentPatients = patients
                .OrderByDescending(p => p.PatientId)
                .Take(5)
                .ToList();

            return View();
        }
    }
}