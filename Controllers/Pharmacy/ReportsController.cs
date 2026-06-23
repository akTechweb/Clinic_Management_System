using System.Text;
using InfinityCoderzz_CMSV2026.Filters;
using InfinityCoderzz_CMSV2026.Services.PharmacyServices;
using Microsoft.AspNetCore.Mvc;

namespace InfinityCoderzz_CMSV2026.Controllers.Pharmacy
{
    [PharmacistAuthorize]
    public class ReportsController : Controller
    {
        private readonly IReportService _service;

        public ReportsController(IReportService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult Index(string report = "sales", DateTime? fromDate = null,
            DateTime? toDate = null, int days = 30)
        {
            ViewData["ActiveMenu"] = "Reports";
            ViewBag.Report = report;
            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");
            ViewBag.Days = days;

            switch (report)
            {
                case "medicinewise":
                    ViewBag.Data = _service.GetMedicineWiseSales(fromDate, toDate);
                    break;
                case "stock":
                    ViewBag.Data = _service.GetStockStatus();
                    break;
                case "expiry":
                    ViewBag.Data = _service.GetExpiryReport(days);
                    break;
                case "lowstock":
                    ViewBag.Data = _service.GetLowStockReport();
                    break;
                case "dispensing":
                    ViewBag.Data = _service.GetDispensingReport(fromDate, toDate);
                    break;
                default:
                    ViewBag.Report = "sales";
                    ViewBag.Data = _service.GetSalesSummary(fromDate, toDate);
                    break;
            }

            return View();
        }

        [HttpGet]
        public IActionResult ExportCsv(string report = "sales", DateTime? fromDate = null,
            DateTime? toDate = null, int days = 30)
        {
            string csv;
            string fileName;

            switch (report)
            {
                case "medicinewise":
                    csv = ToCsv(new[] { "MedicineId", "MedicineName", "QuantitySold", "TotalAmount" },
                        _service.GetMedicineWiseSales(fromDate, toDate)
                            .Select(r => new[] { r.MedicineId.ToString(), r.MedicineName, r.QuantitySold.ToString(), r.TotalAmount.ToString("0.00") }));
                    fileName = "medicine-wise-sales.csv";
                    break;
                case "stock":
                    csv = ToCsv(new[] { "MedicineCode", "MedicineName", "ReorderLevel", "TotalQuantity", "StockStatus" },
                        _service.GetStockStatus()
                            .Select(r => new[] { r.MedicineCode, r.MedicineName, r.ReorderLevel.ToString(), r.TotalQuantity.ToString(), r.StockStatus }));
                    fileName = "stock-status.csv";
                    break;
                case "expiry":
                    csv = ToCsv(new[] { "MedicineCode", "MedicineName", "BatchNumber", "Quantity", "ExpiryDate", "DaysRemaining", "ExpiryStatus" },
                        _service.GetExpiryReport(days)
                            .Select(r => new[] { r.MedicineCode, r.MedicineName, r.BatchNumber, r.Quantity.ToString(), r.ExpiryDate.ToString("yyyy-MM-dd"), r.DaysRemaining.ToString(), r.ExpiryStatus }));
                    fileName = "expiry-report.csv";
                    break;
                case "lowstock":
                    csv = ToCsv(new[] { "MedicineCode", "MedicineName", "ReorderLevel", "TotalQuantity" },
                        _service.GetLowStockReport()
                            .Select(r => new[] { r.MedicineCode, r.MedicineName, r.ReorderLevel.ToString(), r.TotalQuantity.ToString() }));
                    fileName = "low-stock-report.csv";
                    break;
                case "dispensing":
                    csv = ToCsv(new[] { "DispenseId", "PrescriptionId", "PatientName", "PharmacistName", "DispenseDate", "TotalItems", "TotalAmount" },
                        _service.GetDispensingReport(fromDate, toDate)
                            .Select(r => new[] { r.DispenseId.ToString(), r.PrescriptionId.ToString(), r.PatientName, r.PharmacistName, r.DispenseDate.ToString("yyyy-MM-dd HH:mm"), r.TotalItems.ToString(), r.TotalAmount.ToString("0.00") }));
                    fileName = "dispensing-report.csv";
                    break;
                default:
                    csv = ToCsv(new[] { "SaleDate", "BillCount", "ItemsSold", "TotalAmount" },
                        _service.GetSalesSummary(fromDate, toDate)
                            .Select(r => new[] { r.SaleDate.ToString("yyyy-MM-dd"), r.BillCount.ToString(), r.ItemsSold.ToString(), r.TotalAmount.ToString("0.00") }));
                    fileName = "sales-summary.csv";
                    break;
            }

            byte[] bytes = Encoding.UTF8.GetBytes(csv);
            return File(bytes, "text/csv", fileName);
        }

        private static string ToCsv(string[] headers, IEnumerable<string?[]> rows)
        {
            StringBuilder sb = new();
            sb.AppendLine(string.Join(",", headers.Select(Escape)));
            foreach (var row in rows)
                sb.AppendLine(string.Join(",", row.Select(Escape)));
            return sb.ToString();
        }

        private static string Escape(string? value)
        {
            value ??= string.Empty;
            if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
                return "\"" + value.Replace("\"", "\"\"") + "\"";
            return value;
        }
    }
}
