using InfinityCoderzz_CMSV2026.Models.pharmacist;

namespace InfinityCoderzz_CMSV2026.Repositories.PharmacistRepositories
{
    public interface IReportRepository
    {
        IEnumerable<SalesSummaryRow> GetSalesSummary(DateTime? fromDate, DateTime? toDate);
        IEnumerable<MedicineWiseSalesRow> GetMedicineWiseSales(DateTime? fromDate, DateTime? toDate);
        IEnumerable<StockStatusRow> GetStockStatus();
        IEnumerable<ExpiryReportRow> GetExpiryReport(int days);
        IEnumerable<LowStockReportRow> GetLowStockReport();
        IEnumerable<DispensingReportRow> GetDispensingReport(DateTime? fromDate, DateTime? toDate);
    }
}
