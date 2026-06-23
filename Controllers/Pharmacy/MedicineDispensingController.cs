using InfinityCoderzz_CMSV2026.Filters;
using InfinityCoderzz_CMSV2026.Helpers;
using InfinityCoderzz_CMSV2026.Models.pharmacist;
using InfinityCoderzz_CMSV2026.Services.PharmacyServices;
using Microsoft.AspNetCore.Mvc;

namespace InfinityCoderzz_CMSV2026.Controllers.Pharmacy
{
    [PharmacistAuthorize]
    public class MedicineDispensingController : Controller
    {
        private readonly IMedicineDispensingService _service;

        public MedicineDispensingController(IMedicineDispensingService service)
        {
            _service = service;
        }

        #region Dispense Form GET (prescriptions awaiting dispensing)

        [HttpGet]
        public IActionResult Create()
        {
            ViewData["ActiveMenu"] = "Dispensing";
            var prescriptions = _service.GetDispensablePrescriptions();
            return View(prescriptions);
        }

        #endregion

        #region Dispense a whole prescription and auto-generate its bill

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DispenseAndBill(int prescriptionId, string? remarks)
        {
            int pharmacistId = HttpContext.Session.GetInt32("PharmacistId") ?? 0;

            if (prescriptionId <= 0)
            {
                TempData["Error"] = "Invalid prescription selected.";
                return RedirectToAction(nameof(Create));
            }

            try
            {
                DispenseBillResult result = _service.DispenseAndBill(prescriptionId, pharmacistId, remarks);
                TempData["Success"] =
                    $"Prescription dispensed and bill {RefNo.Bill(result.BillId)} generated automatically. " +
                    $"Stock updated (Dispense {RefNo.Dispense(result.DispenseId)}).";
                return RedirectToAction("Details", "Bill", new { id = result.BillId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to dispense prescription. " + ex.Message;
                return RedirectToAction(nameof(Create));
            }
        }

        #endregion

        #region Dispensing History

        [HttpGet]
        public IActionResult History()
        {
            ViewData["ActiveMenu"] = "Dispensing";
            var history = _service.GetDispensingHistory();
            return View(history);
        }

        #endregion

        #region Dispensing Items (AJAX / partial)

        [HttpGet]
        public IActionResult Items(int dispenseId)
        {
            var items = _service.GetDispensingItems(dispenseId);
            return PartialView("_DispensingItems", items);
        }

        #endregion
    }
}
