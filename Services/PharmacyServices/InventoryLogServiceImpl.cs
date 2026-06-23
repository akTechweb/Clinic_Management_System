using InfinityCoderzz_CMSV2026.Models.pharmacist;
using InfinityCoderzz_CMSV2026.Repositories.PharmacistRepositories;



namespace InfinityCoderzz_CMSV2026.Services.PharmacyServices
    {
        public class InventoryLogServiceImpl
            : IInventoryLogService
        {
            private readonly
                IInventoryLogRepository
                _repository;

            public InventoryLogServiceImpl(
                IInventoryLogRepository repository)
            {
                _repository = repository;
            }

            public IEnumerable<MedicineInventoryLog>
                GetInventoryLogs()
            {
                return _repository
                    .GetInventoryLogs();
            }
        }
    



}

