using InfinityCoderzz_CMSV2026.Models.pharmacist;
using InfinityCoderzz_CMSV2026.Services.PharmacyServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;

namespace InfinityCoderzz_CMSV2026.Controllers.Pharmacy
{
    public class MedicineStockController : Controller
    {
        private readonly IMedicineStockService _medicineStockService;

        public MedicineStockController(IMedicineStockService medicineStockService)
        {
            _medicineStockService = medicineStockService;
        }

        #region List Stock

        [HttpGet]
        public IActionResult List()
        {
            int pharmacistId = HttpContext.Session.GetInt32("PharmacistId") ?? 0;
            if (pharmacistId == 0) return RedirectToAction("Index", "Login");

            var stocks = _medicineStockService.GetAllMedicineStock();
            return View(stocks);
        }

        #endregion

        #region Create GET

        [HttpGet]
        public IActionResult Create()
        {
            int pharmacistId = HttpContext.Session.GetInt32("PharmacistId") ?? 0;
            if (pharmacistId == 0) return RedirectToAction("Index", "Login");

            ViewBag.Medicines = new SelectList(
                _medicineStockService.GetAllMedicines(),
                "MedicineId",
                "MedicineName");

            return View();
        }

        #endregion

        #region Create POST

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(MedicineStock stock)
        {
            int pharmacistId = HttpContext.Session.GetInt32("PharmacistId") ?? 0;
            if (pharmacistId == 0) return RedirectToAction("Index", "Login");

            // Never trust a client-supplied identity for a new record.
            stock.StockId = 0;
            stock.BatchNumber = stock.BatchNumber?.Trim();

            // Selected medicine must exist and be active.
            if (!_medicineStockService.GetAllMedicines()
                    .Any(m => m.MedicineId == stock.MedicineId))
            {
                ModelState.AddModelError(
                    nameof(stock.MedicineId),
                    "Selected medicine is not valid or is inactive.");
            }

            // Batch number format — enforced only for NEW batches.
            if (!string.IsNullOrWhiteSpace(stock.BatchNumber)
                && !Regex.IsMatch(stock.BatchNumber, @"^[A-Za-z0-9/\-]+$"))
            {
                ModelState.AddModelError(
                    nameof(stock.BatchNumber),
                    "Batch number may only contain letters, numbers, hyphen (-) and slash (/).");
            }

            // New stock cannot already be expired (or expiring today).
            if (stock.ExpiryDate != default && stock.ExpiryDate.Date <= DateTime.Today)
            {
                ModelState.AddModelError(
                    nameof(stock.ExpiryDate),
                    "Expiry date must be a future date.");
            }

            // Purchase price may not have more than 2 decimal places.
            if (stock.PurchasePrice.HasValue && HasMoreThanTwoDecimals(stock.PurchasePrice.Value))
            {
                ModelState.AddModelError(
                    nameof(stock.PurchasePrice),
                    "Purchase price cannot have more than 2 decimal places.");
            }

            // Batch number must be unique per medicine.
            if (stock.MedicineId > 0 && !string.IsNullOrWhiteSpace(stock.BatchNumber)
                && _medicineStockService.BatchExists(stock.MedicineId, stock.BatchNumber, 0))
            {
                ModelState.AddModelError(
                    nameof(stock.BatchNumber),
                    "This batch number already exists for the selected medicine.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Medicines = new SelectList(
                    _medicineStockService.GetAllMedicines(),
                    "MedicineId",
                    "MedicineName");

                return View(stock);
            }

            // Default the purchase date to today when not supplied.
            if (stock.PurchaseDate == null)
                stock.PurchaseDate = DateTime.Today;

            try
            {
                _medicineStockService.AddMedicineStock(stock);
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                // Concurrency safety-net: the DB unique constraint on
                // (MedicineId, BatchNumber) rejected a duplicate that slipped past the
                // app-level check. Surface a friendly message instead of a 500.
                ModelState.AddModelError(
                    nameof(stock.BatchNumber),
                    "This batch number already exists for the selected medicine.");

                ViewBag.Medicines = new SelectList(
                    _medicineStockService.GetAllMedicines(),
                    "MedicineId",
                    "MedicineName");

                return View(stock);
            }

            return RedirectToAction(nameof(List));
        }

        #endregion

        #region Edit GET

        [HttpGet]
        public IActionResult Edit(int id)
        {
            int pharmacistId = HttpContext.Session.GetInt32("PharmacistId") ?? 0;
            if (pharmacistId == 0) return RedirectToAction("Index", "Login");

            var stock = _medicineStockService.GetMedicineStockById(id);

            if (stock == null)
                return NotFound();

            return View(stock);
        }

        #endregion

        #region Edit POST

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(MedicineStock stock)
        {
            int pharmacistId = HttpContext.Session.GetInt32("PharmacistId") ?? 0;
            if (pharmacistId == 0) return RedirectToAction("Index", "Login");

            stock.BatchNumber = stock.BatchNumber?.Trim();

            // Load the existing row to validate against its stored values.
            var existing = _medicineStockService.GetMedicineStockById(stock.StockId);
            if (existing == null)
                return NotFound();

            // Batch number must remain unique per medicine (excluding this row).
            if (stock.MedicineId > 0 && !string.IsNullOrWhiteSpace(stock.BatchNumber)
                && _medicineStockService.BatchExists(stock.MedicineId, stock.BatchNumber, stock.StockId))
            {
                ModelState.AddModelError(
                    nameof(stock.BatchNumber),
                    "This batch number already exists for the selected medicine.");
            }

            // Purchase price may not have more than 2 decimal places.
            if (stock.PurchasePrice.HasValue && HasMoreThanTwoDecimals(stock.PurchasePrice.Value))
            {
                ModelState.AddModelError(
                    nameof(stock.PurchasePrice),
                    "Purchase price cannot have more than 2 decimal places.");
            }

            // Expiry must stay after the batch's recorded purchase date.
            if (stock.ExpiryDate != default && existing.PurchaseDate.HasValue
                && stock.ExpiryDate.Date <= existing.PurchaseDate.Value.Date)
            {
                ModelState.AddModelError(
                    nameof(stock.ExpiryDate),
                    "Expiry date must be after the purchase date.");
            }

            // You may keep an already-past expiry, but you cannot CHANGE the expiry
            // to a new past (or today) date.
            if (stock.ExpiryDate != default
                && stock.ExpiryDate.Date != existing.ExpiryDate.Date
                && stock.ExpiryDate.Date <= DateTime.Today)
            {
                ModelState.AddModelError(
                    nameof(stock.ExpiryDate),
                    "Expiry date cannot be changed to a past date.");
            }

            if (!ModelState.IsValid)
                return View(stock);

            _medicineStockService.UpdateMedicineStock(stock);
            return RedirectToAction(nameof(List));
        }

        #endregion

        #region Low Stock

        [HttpGet]
        public IActionResult LowStock()
        {
            int pharmacistId = HttpContext.Session.GetInt32("PharmacistId") ?? 0;
            if (pharmacistId == 0) return RedirectToAction("Index", "Login");

            var stocks = _medicineStockService.GetLowStockMedicines();
            return View(stocks);
        }

        #endregion

        #region Expiring Medicines

        [HttpGet]
        public IActionResult ExpiringMedicines()
        {
            int pharmacistId = HttpContext.Session.GetInt32("PharmacistId") ?? 0;
            if (pharmacistId == 0) return RedirectToAction("Index", "Login");

            var medicines = _medicineStockService.GetExpiringMedicines();
            return View(medicines);
        }

        #endregion

        #region Expired Medicines

        [HttpGet]
        public IActionResult ExpiredMedicines()
        {
            int pharmacistId = HttpContext.Session.GetInt32("PharmacistId") ?? 0;
            if (pharmacistId == 0) return RedirectToAction("Index", "Login");

            var medicines = _medicineStockService.GetExpiredMedicines();
            return View(medicines);
        }

        #endregion

        #region Helpers

        // True when the value carries more than 2 decimal places (e.g. 12.345).
        private static bool HasMoreThanTwoDecimals(decimal value)
            => value != decimal.Round(value, 2, MidpointRounding.AwayFromZero);

        #endregion
    }
}
