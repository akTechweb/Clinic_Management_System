using InfinityCoderzz_CMSV2026.Models.pharmacist;
using InfinityCoderzz_CMSV2026.Services.PharmacyServices;
using InfinityCoderzz_CMSV2026.Services.PharmacyServices.Pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuestPDF.Fluent;

namespace InfinityCoderzz_CMSV2026.Controllers
{
    public class BillController : Controller
    {
        private readonly IPharmacyBillService _billService;

        public BillController(IPharmacyBillService billService)
        {
            _billService = billService;
        }

        #region Auth Helper

        private bool IsPharmacistLoggedIn()
            => (HttpContext.Session.GetInt32("PharmacistId") ?? 0) > 0;

        private IActionResult AuthRedirect()
            => RedirectToAction("Index", "Login");

        #endregion

        #region Bill List

        [HttpGet]
        public IActionResult List()
        {
            if (!IsPharmacistLoggedIn()) return AuthRedirect();
            ViewData["ActiveMenu"] = "Bills";
            var bills = _billService.GetAllBills();
            return View(bills);
        }

        #endregion

        #region Create Bill GET

        [HttpGet]
        public IActionResult Create()
        {
            if (!IsPharmacistLoggedIn()) return AuthRedirect();
            ViewData["ActiveMenu"] = "Bills";

            ViewBag.Patients = new SelectList(
                _billService.GetPatients()
                    .Select(p => new { p.PatientId, Display = $"{p.PatientCode} - {p.FullName}" }),
                "PatientId", "Display");

            ViewBag.Medicines = _billService.GetMedicinesForBilling();
            return View(new CreateBillViewModel());
        }

        #endregion

        #region Create Bill POST

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateBillViewModel model)
        {
            if (!IsPharmacistLoggedIn()) return AuthRedirect();

            int pharmacistId = HttpContext.Session.GetInt32("PharmacistId") ?? 0;

            if (model.BillItems == null || !model.BillItems.Any())
            {
                TempData["Error"] = "Please add at least one medicine to the bill.";
                return RedirectToAction(nameof(Create));
            }

            try
            {
                int billId = _billService.CreateBill(model, pharmacistId);
                TempData["Success"] = "Bill created successfully.";
                return RedirectToAction(nameof(Details), new { id = billId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to create bill. " + ex.Message;
                ViewBag.Patients = new SelectList(
                    _billService.GetPatients()
                        .Select(p => new { p.PatientId, Display = $"{p.PatientCode} - {p.FullName}" }),
                    "PatientId", "Display");
                ViewBag.Medicines = _billService.GetMedicinesForBilling();
                return View(model);
            }
        }

        #endregion

        #region Bill Details

        [HttpGet]
        public IActionResult Details(int id)
        {
            if (!IsPharmacistLoggedIn()) return AuthRedirect();
            ViewData["ActiveMenu"] = "Bills";

            var bill = _billService.GetBillById(id);
            if (bill == null) return NotFound();

            ViewBag.Items = _billService.GetBillItems(id);
            ViewBag.PrescriptionLink = _billService.GetBillPrescriptionLink(id);
            return View(bill);
        }

        #endregion

        #region Cancel Bill

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cancel(int id, string? reason)
        {
            if (!IsPharmacistLoggedIn()) return AuthRedirect();

            int pharmacistId = HttpContext.Session.GetInt32("PharmacistId") ?? 0;

            try
            {
                _billService.CancelBill(id, pharmacistId, reason);
                TempData["Success"] = "Bill cancelled successfully. Stock has been restored.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to cancel bill. " + ex.Message;
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        #endregion

        #region Printable Invoice (HTML)

        [HttpGet]
        public IActionResult Invoice(int id)
        {
            if (!IsPharmacistLoggedIn()) return AuthRedirect();

            var bill = _billService.GetBillById(id);
            if (bill == null) return NotFound();

            ViewBag.Items = _billService.GetBillItems(id);
            return View(bill);
        }

        #endregion

        #region Invoice PDF (QuestPDF)

        [HttpGet]
        public IActionResult InvoicePdf(int id)
        {
            if (!IsPharmacistLoggedIn()) return AuthRedirect();

            var bill = _billService.GetBillById(id);
            if (bill == null) return NotFound();

            var items = _billService.GetBillItems(id);
            var document = new InvoiceDocument(bill, items);
            byte[] pdfBytes = document.GeneratePdf();

            return File(pdfBytes, "application/pdf", $"Invoice-{id}.pdf");
        }

        #endregion
    }
}
