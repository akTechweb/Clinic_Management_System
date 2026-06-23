using InfinityCoderzz_CMSV2026.Models.pharmacist;
using InfinityCoderzz_CMSV2026.Repositories.PharmacistRepositories;

namespace InfinityCoderzz_CMSV2026.Services.PharmacyServices
{
    public class MedicineStockServiceImpl : IMedicineStockService
    {
        private readonly IMedicineStockRepository _medicineStockRepository;

        public MedicineStockServiceImpl(IMedicineStockRepository medicineStockRepository)
        {
            _medicineStockRepository = medicineStockRepository;
        }

        public IEnumerable<MedicineStock> GetAllMedicineStock()
            => _medicineStockRepository.GetAllMedicineStock();

        public MedicineStock GetMedicineStockById(int stockId)
            => _medicineStockRepository.GetMedicineStockById(stockId);

        public void AddMedicineStock(MedicineStock stock)
            => _medicineStockRepository.AddMedicineStock(stock);

        public void UpdateMedicineStock(MedicineStock stock)
            => _medicineStockRepository.UpdateMedicineStock(stock);

        public IEnumerable<MedicineStock> GetLowStockMedicines()
            => _medicineStockRepository.GetLowStockMedicines();

        public IEnumerable<Medicine> GetAllMedicines()
            => _medicineStockRepository.GetAllMedicines();

        public IEnumerable<MedicineStock> GetExpiredMedicines()
            => _medicineStockRepository.GetExpiredMedicines();

        public IEnumerable<MedicineStock> GetExpiringMedicines()
            => _medicineStockRepository.GetExpiringMedicines();

        public bool BatchExists(int medicineId, string batchNumber, int excludeStockId)
            => _medicineStockRepository.BatchExists(medicineId, batchNumber, excludeStockId);
    }
}
