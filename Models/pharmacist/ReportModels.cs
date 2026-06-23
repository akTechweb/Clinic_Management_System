namespace InfinityCoderzz_CMSV2026.Models.pharmacist
{
    public class SalesSummaryRow
    {
        public DateTime SaleDate { get; set; }
        public int BillCount { get; set; }
        public int ItemsSold { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class MedicineWiseSalesRow
    {
        public int MedicineId { get; set; }
        public string? MedicineName { get; set; }
        public int QuantitySold { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class StockStatusRow
    {
        public int MedicineId { get; set; }
        public string? MedicineCode { get; set; }
        public string? MedicineName { get; set; }
        public int ReorderLevel { get; set; }
        public int TotalQuantity { get; set; }
        public string? StockStatus { get; set; }
    }

    public class ExpiryReportRow
    {
        public int StockId { get; set; }
        public string? MedicineCode { get; set; }
        public string? MedicineName { get; set; }
        public string? BatchNumber { get; set; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int DaysRemaining { get; set; }
        public string? ExpiryStatus { get; set; }
    }

    public class LowStockReportRow
    {
        public int MedicineId { get; set; }
        public string? MedicineCode { get; set; }
        public string? MedicineName { get; set; }
        public int ReorderLevel { get; set; }
        public int TotalQuantity { get; set; }
    }

    public class DispensingReportRow
    {
        public int DispenseId { get; set; }
        public int PrescriptionId { get; set; }
        public string? PatientName { get; set; }
        public string? PharmacistName { get; set; }
        public DateTime DispenseDate { get; set; }
        public int TotalItems { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
