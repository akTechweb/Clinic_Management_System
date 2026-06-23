using InfinityCoderzz_CMSV2026.Services.PharmacyServices;
using Microsoft.AspNetCore.Mvc;

namespace InfinityCoderzz_CMSV2026.Controllers.Pharmacy
{
    public class PrescriptionController : Controller
    {
        private readonly IPrescriptionService _prescriptionService;

        public PrescriptionController(IPrescriptionService prescriptionService)
        {
            _prescriptionService = prescriptionService;
        }

        #region Auth Helper

        private bool IsPharmacistLoggedIn()
            => (HttpContext.Session.GetInt32("PharmacistId") ?? 0) > 0;

        private IActionResult AuthRedirect()
            => RedirectToAction("Index", "Login");

        #endregion

        #region List Prescriptions

        [HttpGet]
        public IActionResult List()
        {
            if (!IsPharmacistLoggedIn()) return AuthRedirect();
            ViewData["ActiveMenu"] = "Prescriptions";
            return View(_prescriptionService.GetAllPrescriptions());
        }

        #endregion

        #region Prescription Details

        [HttpGet]
        public IActionResult Details(int id)
        {
            if (!IsPharmacistLoggedIn()) return AuthRedirect();
            ViewData["ActiveMenu"] = "Prescriptions";

            var prescription = _prescriptionService.GetPrescriptionById(id);
            if (prescription == null) return NotFound();

            ViewBag.Items = _prescriptionService.GetPrescriptionItems(id);
            return View(prescription);
        }

        #endregion

        #region Mark Dispensed

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MarkDispensed(int id)
        {
            if (!IsPharmacistLoggedIn()) return AuthRedirect();

            try
            {
                _prescriptionService.UpdatePrescriptionStatus(id, "Dispensed");
                TempData["Success"] = "Prescription marked as Dispensed.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Could not update prescription status. " + ex.Message;
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        #endregion

        #region Update Status

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateStatus(int id, string status)
        {
            if (!IsPharmacistLoggedIn()) return AuthRedirect();

            if (string.IsNullOrWhiteSpace(status))
            {
                TempData["Error"] = "Status cannot be empty.";
                return RedirectToAction(nameof(Details), new { id });
            }

            try
            {
                _prescriptionService.UpdatePrescriptionStatus(id, status);
                TempData["Success"] = $"Prescription status updated to '{status}'.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to update status. " + ex.Message;
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        #endregion
    }
}
