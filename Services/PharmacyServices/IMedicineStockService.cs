using InfinityCoderzz_CMSV2026.Models.pharmacist;

namespace InfinityCoderzz_CMSV2026.Services.PharmacyServices
{
    public interface IMedicineStockService
    {
        IEnumerable<MedicineStock> GetAllMedicineStock();

        MedicineStock GetMedicineStockById(int stockId);

        void AddMedicineStock(MedicineStock stock);

        void UpdateMedicineStock(MedicineStock stock);

        IEnumerable<MedicineStock> GetLowStockMedicines();

        IEnumerable<Medicine> GetAllMedicines();

        IEnumerable<MedicineStock> GetExpiringMedicines();

        IEnumerable<MedicineStock> GetExpiredMedicines();

        bool BatchExists(int medicineId, string batchNumber, int excludeStockId);
    }
}
