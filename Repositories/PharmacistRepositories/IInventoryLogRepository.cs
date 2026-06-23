using InfinityCoderzz_CMSV2026.Models.pharmacist;



namespace InfinityCoderzz_CMSV2026.Repositories.PharmacistRepositories
{
        public interface IInventoryLogRepository
        {
            IEnumerable<MedicineInventoryLog>
                GetInventoryLogs();
        }
}


