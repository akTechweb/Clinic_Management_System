using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InfinityCoderzz_CMSV2026.Models;
using InfinityCoderzz_CMSV2026.Services;

namespace InfinityCoderzz_CMSV2026.Controllers
{
    public class DoctorController : Controller
    {
        private readonly IDoctorService _svc;
        public DoctorController(IDoctorService svc) => _svc = svc;

        private int DoctorId => HttpContext.Session.GetInt32("DoctorId") ?? 0;

        [HttpGet]
        public IActionResult Login()
        {
            if (DoctorId != 0) return RedirectToAction("Dashboard");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // ModelState.IsValid check இல்லாம directly authenticate பண்ணுங்க
            var session = await _svc.AuthenticateDoctorAsync(model);

            if (session != null)
            {
                HttpContext.Session.SetInt32("DoctorId", session.DoctorId);
                HttpContext.Session.SetInt32("UserId", session.UserId);
                HttpContext.Session.SetString("FullName", session.FullName ?? "Doctor");
                HttpContext.Session.SetString("RoleName", session.RoleName ?? "Doctor");
                return RedirectToAction("Dashboard");
            }

            ModelState.AddModelError("", "Invalid username or password.");
            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }

        public async Task<IActionResult> Dashboard()
        {
            if (DoctorId == 0) return RedirectToAction("Index", "Login");
            var stats = await _svc.GetDashboardStatsAsync(DoctorId);
            ViewBag.DoctorName = HttpContext.Session.GetString("FullName") ?? "Doctor";
            return View(stats);
        }

        public async Task<IActionResult> ViewAppointments(string targetDay = "today")
        {
            if (DoctorId == 0) return RedirectToAction("Index", "Login");
            List<AppointmentViewModel> list;
            if (targetDay == "tomorrow")
                list = await _svc.GetTomorrowAppointmentsAsync(DoctorId);
            else
                list = await _svc.GetTodaysAppointmentsAsync(DoctorId);
            ViewBag.CurrentSelection = targetDay;
            return View(list);
        }

        public async Task<IActionResult> StartConsultation(int appointmentId)
        {
            if (DoctorId == 0) return RedirectToAction("Index", "Login");
            var vm = await _svc.GetConsultationSetupDataAsync(appointmentId);
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitConsultation(ConsultationSetupViewModel model)
        {
            if (DoctorId == 0) return RedirectToAction("Index", "Login");

            if (false && !ModelState.IsValid)
            {
                var fresh = await _svc.GetConsultationSetupDataAsync(model.AppointmentId);
                model.AvailableMedicines = fresh.AvailableMedicines;
                model.AvailableLabTests = fresh.AvailableLabTests;
                model.HistoricalData = fresh.HistoricalData;
                return View("StartConsultation", model);
            }

            var summary = await _svc.SaveFullConsultationAsync(model, DoctorId);

            TempData["Summary"] = System.Text.Json.JsonSerializer.Serialize(summary);
            return RedirectToAction("ConsultationOutcomeSummary");
        }

        public IActionResult ConsultationOutcomeSummary()
        {
            if (DoctorId == 0) return RedirectToAction("Index", "Login");
            if (TempData["Summary"] is string json)
            {
                var vm = System.Text.Json.JsonSerializer.Deserialize<FinalSummaryDocumentViewModel>(json);
                return View(vm);
            }
            return RedirectToAction("Dashboard");
        }

        public async Task<IActionResult> HistoryAndReports(string searchKeyword = "")
        {
            if (DoctorId == 0) return RedirectToAction("Index", "Login");
            var list = await _svc.SearchPatientsAsync(searchKeyword);
            ViewBag.SearchKeyword = searchKeyword;
            return View(list);
        }

        public async Task<IActionResult> ViewLabReportDetails(string mmrCode)
        {
            if (DoctorId == 0) return RedirectToAction("Index", "Login");
            var report = await _svc.GetPatientFullReportAsync(mmrCode);
            return PartialView("_LabReportsTable", report);
        }

        public async Task<IActionResult> PrintReport(string mmrCode)
        {
            if (DoctorId == 0) return RedirectToAction("Index", "Login");
            var report = await _svc.GetPatientFullReportAsync(mmrCode);
            return View("PrintReport", report);
        }
    }
}