using InfinityCoderzz_CMSV2026.Models.pharmacist;
using InfinityCoderzz_CMSV2026.Services.PharmacyServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InfinityCoderzz_CMSV2026.Controllers.Pharmacy
{
    public class MedicinesController : Controller
    {
        private readonly IMedicineService _medicineService;

        public MedicinesController(IMedicineService medicineService)
        {
            _medicineService = medicineService;
        }

        #region List Medicines

        [HttpGet]
        public IActionResult List(string searchTerm)
        {
            int pharmacistId = HttpContext.Session.GetInt32("PharmacistId") ?? 0;
            if (pharmacistId == 0) return RedirectToAction("Index", "Login");

            IEnumerable<Medicine> medicines = !string.IsNullOrWhiteSpace(searchTerm)
                ? _medicineService.SearchMedicine(searchTerm)
                : _medicineService.GetAllMedicines();

            ViewBag.SearchTerm = searchTerm;
            return View(medicines);
        }

        #endregion

        #region Create Medicine GET

        [HttpGet]
        public IActionResult Create()
        {
            int pharmacistId = HttpContext.Session.GetInt32("PharmacistId") ?? 0;
            if (pharmacistId == 0) return RedirectToAction("Index", "Login");

            ViewBag.Categories = new SelectList(
                _medicineService.GetAllCategories(), "CategoryId", "CategoryName");

            ViewBag.Manufacturers = new SelectList(
                _medicineService.GetAllManufacturers(), "ManufacturerId", "ManufacturerName");

            var model = new Medicine
            {
                MedicineCode = _medicineService.GenerateNextMedicineCode()
            };

            return View(model);
        }

        #endregion

        #region Create Medicine POST

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Medicine medicine)
        {
            int pharmacistId = HttpContext.Session.GetInt32("PharmacistId") ?? 0;
            if (pharmacistId == 0) return RedirectToAction("Index", "Login");

            try
            {
                if (string.IsNullOrWhiteSpace(medicine.MedicineCode))
                {
                    medicine.MedicineCode = _medicineService.GenerateNextMedicineCode();
                    ModelState.Remove(nameof(Medicine.MedicineCode));
                }
                else
                {
                    medicine.MedicineCode = medicine.MedicineCode.Trim();

                    if (!_medicineService.IsMedicineCodeUnique(medicine.MedicineCode))
                    {
                        ModelState.AddModelError(
                            nameof(Medicine.MedicineCode),
                            "This medicine code is already in use. Leave it blank to auto-generate.");
                    }
                }

                if (ModelState.IsValid)
                {
                    _medicineService.AddMedicine(medicine);
                    TempData["Success"] = "Medicine added successfully.";
                    return RedirectToAction(nameof(List));
                }

                ViewBag.Categories = new SelectList(
                    _medicineService.GetAllCategories(), "CategoryId", "CategoryName");

                ViewBag.Manufacturers = new SelectList(
                    _medicineService.GetAllManufacturers(), "ManufacturerId", "ManufacturerName");

                return View(medicine);
            }
            catch
            {
                TempData["Error"] = "Failed to add medicine. Please try again.";
                ViewBag.Categories = new SelectList(
                    _medicineService.GetAllCategories(), "CategoryId", "CategoryName");
                ViewBag.Manufacturers = new SelectList(
                    _medicineService.GetAllManufacturers(), "ManufacturerId", "ManufacturerName");
                return View(medicine);
            }
        }

        #endregion

        #region Edit Medicine GET

        [HttpGet]
        public IActionResult Edit(int id)
        {
            int pharmacistId = HttpContext.Session.GetInt32("PharmacistId") ?? 0;
            if (pharmacistId == 0) return RedirectToAction("Index", "Login");

            Medicine medicine = _medicineService.GetMedicineById(id);

            if (medicine == null)
                return NotFound();

            ViewBag.Categories = new SelectList(
                _medicineService.GetAllCategories(), "CategoryId", "CategoryName", medicine.CategoryId);

            ViewBag.Manufacturers = new SelectList(
                _medicineService.GetAllManufacturers(), "ManufacturerId", "ManufacturerName", medicine.ManufacturerId);

            return View(medicine);
        }

        #endregion

        #region Edit Medicine POST

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Medicine medicine)
        {
            int pharmacistId = HttpContext.Session.GetInt32("PharmacistId") ?? 0;
            if (pharmacistId == 0) return RedirectToAction("Index", "Login");

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(
                    _medicineService.GetAllCategories(), "CategoryId", "CategoryName", medicine.CategoryId);

                ViewBag.Manufacturers = new SelectList(
                    _medicineService.GetAllManufacturers(), "ManufacturerId", "ManufacturerName", medicine.ManufacturerId);

                return View(medicine);
            }

            _medicineService.UpdateMedicine(medicine);
            TempData["Success"] = "Medicine updated successfully.";
            return RedirectToAction(nameof(List));
        }

        #endregion

        #region Disable Medicine

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Disable(int id)
        {
            int pharmacistId = HttpContext.Session.GetInt32("PharmacistId") ?? 0;
            if (pharmacistId == 0) return RedirectToAction("Index", "Login");

            _medicineService.DisableMedicine(id);
            TempData["Success"] = "Medicine disabled.";
            return RedirectToAction(nameof(List));
        }

        #endregion
    }
}
