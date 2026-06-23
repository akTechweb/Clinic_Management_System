using InfinityCoderzz_CMSV2026.Models.pharmacist;
using InfinityCoderzz_CMSV2026.Repositories.PharmacistRepositories;

namespace InfinityCoderzz_CMSV2026.Services.PharmacyServices
{
    public class ReportServiceImpl : IReportService
    {
        private readonly IReportRepository _repository;

        public ReportServiceImpl(IReportRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<SalesSummaryRow> GetSalesSummary(DateTime? fromDate, DateTime? toDate)
            => _repository.GetSalesSummary(fromDate, toDate);

        public IEnumerable<MedicineWiseSalesRow> GetMedicineWiseSales(DateTime? fromDate, DateTime? toDate)
            => _repository.GetMedicineWiseSales(fromDate, toDate);

        public IEnumerable<StockStatusRow> GetStockStatus()
            => _repository.GetStockStatus();

        public IEnumerable<ExpiryReportRow> GetExpiryReport(int days)
            => _repository.GetExpiryReport(days);

        public IEnumerable<LowStockReportRow> GetLowStockReport()
            => _repository.GetLowStockReport();

        public IEnumerable<DispensingReportRow> GetDispensingReport(DateTime? fromDate, DateTime? toDate)
            => _repository.GetDispensingReport(fromDate, toDate);
    }
}
