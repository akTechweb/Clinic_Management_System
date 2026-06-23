using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using InfinityCoderzz_CMSV2026.Models;
using InfinityCoderzz_CMSV2026.Services;

namespace InfinityCoderzz_CMSV2026.Controllers
{
    public class LabTechnicianController : Controller
    {
        private readonly ILabTechnicianService _svc;
        public LabTechnicianController(ILabTechnicianService svc) => _svc = svc;

        // Hardcoded technician id (login handled by the shared/common module).
        // LabTechnicians table has a seeded row with TechnicianId = 1 (labtechadmin).
        private const int TechnicianId = 1;

        // ── DASHBOARD ────────────────────────────────────────────────────────
        public async Task<IActionResult> Dashboard()
        {
            var stats = await _svc.GetDashboardStatsAsync();
            ViewBag.TechnicianName = "Lab Admin";
            return View(stats);
        }

        // ── PENDING TESTS (search by MMR) ───────────────────────────────────
        public async Task<IActionResult> PendingTests(string searchMMR = "")
        {
            var list = await _svc.GetPendingLabRequestsAsync(searchMMR);
            ViewBag.SearchMMR = searchMMR;
            return View(list);
        }

        // ── ENTER RESULT (GET) ───────────────────────────────────────────────
        public async Task<IActionResult> EnterResult(int requestItemId)
        {
            var vm = await _svc.GetLabRequestItemDetailsAsync(requestItemId);
            if (vm == null) return NotFound();

            if (vm.Status == "Completed")
            {
                TempData["InfoMessage"] = "Result for this test has already been entered.";
                return RedirectToAction("PendingTests");
            }
            return View(vm);
        }

        // ── ENTER RESULT (POST) ──────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnterResult(EnterLabResultViewModel model)
        {
            // FIX: Log every ModelState error to help diagnose silent failures.
            // If ModelState is invalid the form is returned with visible errors.
            if (!ModelState.IsValid)
            {
                // Re-hydrate display-only fields that are NOT posted back
                var fresh = await _svc.GetLabRequestItemDetailsAsync(model.RequestItemId);
                if (fresh != null)
                {
                    model.TestName = fresh.TestName;
                    model.TestDescription = fresh.TestDescription;
                    model.NormalRange = fresh.NormalRange;
                    model.TestCharge = fresh.TestCharge;   // FIX: was missing in re-hydration
                    model.PatientName = fresh.PatientName;
                    model.Age = fresh.Age;
                    model.Gender = fresh.Gender;
                    model.MobileNumber = fresh.MobileNumber;
                    model.MMRCode = fresh.MMRCode;
                    model.DoctorName = fresh.DoctorName;
                    model.DoctorEmail = fresh.DoctorEmail;
                }

                // Surface ModelState errors as a TempData warning so you can
                // see them even if asp-validation-summary is not rendering.
                var errors = new System.Text.StringBuilder();
                foreach (var kv in ModelState)
                    foreach (var err in kv.Value.Errors)
                        errors.AppendLine($"{kv.Key}: {err.ErrorMessage}");

                TempData["ErrorMessage"] = $"Validation failed — please check the form.\n{errors}";
                return View(model);
            }

            try
            {
                // Save result via stored procedure usp_AddLabResult
                int resultId = await _svc.SaveLabResultAsync(model, TechnicianId);

                if (resultId <= 0)
                {
                    // SP returned 0 or negative — INSERT failed inside the database
                    TempData["ErrorMessage"] = "Database error: Result was not saved. " +
                        "Please verify that usp_AddLabResult exists and all required " +
                        "tables (LabResults, LabRequestItems, LabRequests) are present.";
                    var fresh = await _svc.GetLabRequestItemDetailsAsync(model.RequestItemId);
                    if (fresh != null) { model.TestName = fresh.TestName; model.TestCharge = fresh.TestCharge; }
                    return View(model);
                }

                // Auto-send the result to the assigned doctor's email.
                // Email failure does NOT roll back the saved result.
                bool emailSent = await _svc.SendResultToDoctorAsync(resultId);

                TempData["SuccessMessage"] = emailSent
                    ? "Result saved successfully and emailed to the consulting doctor."
                    : "Result saved successfully. (Email could not be sent — check SMTP settings.)";

                return RedirectToAction("ReportsDashboard");
            }
            catch (Microsoft.Data.SqlClient.SqlException sqlEx)
            {
                // Catches: SP not found, FK violation, column missing, type mismatch, etc.
                TempData["ErrorMessage"] =
                    $"SQL Error {sqlEx.Number}: {sqlEx.Message} — " +
                    "Check that usp_AddLabResult exists and all referenced columns/tables exist.";

                System.Diagnostics.Debug.WriteLine($"[EnterResult SQL ERROR] {sqlEx}");

                var fresh = await _svc.GetLabRequestItemDetailsAsync(model.RequestItemId);
                if (fresh != null) { model.TestName = fresh.TestName; model.TestCharge = fresh.TestCharge; }
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Unexpected error: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"[EnterResult ERROR] {ex}");

                var fresh = await _svc.GetLabRequestItemDetailsAsync(model.RequestItemId);
                if (fresh != null) { model.TestName = fresh.TestName; model.TestCharge = fresh.TestCharge; }
                return View(model);
            }
        }

        // ── REPORTS DASHBOARD ────────────────────────────────────────────────
        public async Task<IActionResult> ReportsDashboard(string searchMMR = "")
        {
            var list = await _svc.GetCompletedLabReportsAsync(searchMMR);
            ViewBag.SearchMMR = searchMMR;
            return View(list);
        }

        // ── PRINT / DOWNLOAD PDF REPORT ──────────────────────────────────────
        public async Task<IActionResult> DownloadReportPdf(int resultId)
        {
            var r = await _svc.GetLabResultDetailsAsync(resultId);
            if (r == null) return NotFound();
            return View("PrintReport", r);
        }

        // ── RE-SEND EMAIL TO DOCTOR ──────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendEmail(int resultId, string searchMMR = "")
        {
            bool sent = await _svc.SendResultToDoctorAsync(resultId);
            TempData[sent ? "SuccessMessage" : "ErrorMessage"] =
                sent ? "Report emailed to doctor successfully."
                     : "Failed to send email. Please check the doctor's email address and SMTP settings.";
            return RedirectToAction("ReportsDashboard", new { searchMMR });
        }

        // ── BILLING DASHBOARD ────────────────────────────────────────────────
        public async Task<IActionResult> BillingDashboard(string searchMMR = "")
        {
            var unbilled = await _svc.GetUnbilledLabRequestsAsync(searchMMR);
            var bills = await _svc.GetLabBillsAsync(searchMMR);
            ViewBag.SearchMMR = searchMMR;
            ViewBag.Bills = bills;
            return View(unbilled);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateBill(int requestId, string searchMMR = "")
        {
            try
            {
                int billId = await _svc.GenerateLabBillAsync(requestId, TechnicianId);
                TempData["SuccessMessage"] = $"Bill #{billId} generated successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Failed to generate bill: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"[GenerateBill ERROR] {ex}");
            }
            return RedirectToAction("BillingDashboard", new { searchMMR });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateBillPayment(int billId, string paymentStatus, string searchMMR = "")
        {
            await _svc.UpdateLabBillPaymentStatusAsync(billId, paymentStatus);
            TempData["SuccessMessage"] = "Payment status updated.";
            return RedirectToAction("BillingDashboard", new { searchMMR });
        }

        public async Task<IActionResult> PrintBill(int billId)
        {
            var vm = await _svc.GetLabBillDetailsAsync(billId);
            if (vm == null || vm.Bill == null) return NotFound();
            return View("PrintBill", vm);
        }

        public async Task<IActionResult> DownloadBillPdf(int billId)
        {
            try
            {
                var vm = await _svc.GetLabBillDetailsAsync(billId);
                if (vm == null || vm.Bill == null) return NotFound();

                var pdfStream = await _svc.GenerateBillPdfAsync(billId);
                string filename = $"Bill_{vm.Bill.MMRCode}_{vm.Bill.BillId}.pdf";
                return File(pdfStream, "application/pdf", filename);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Failed to generate PDF: {ex.Message}";
                return RedirectToAction("BillingDashboard");
            }
        }

        // ── PATIENT SEARCH (AJAX, MMR autocomplete) ──────────────────────────
        [HttpGet]
        public async Task<IActionResult> SearchPatientByMMR(string term)
        {
            var list = await _svc.SearchPatientByMMRAsync(term);
            return Json(list);
        }
    }
}
