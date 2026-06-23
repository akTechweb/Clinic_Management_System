using InfinityCoderzz_CMSV2026.Models.pharmacist;

namespace InfinityCoderzz_CMSV2026.Services.PharmacyServices
{
    public interface IInventoryLogService
    {
        IEnumerable<MedicineInventoryLog>
            GetInventoryLogs();
    }
}