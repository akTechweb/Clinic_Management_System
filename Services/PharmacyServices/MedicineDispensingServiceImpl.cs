using InfinityCoderzz_CMSV2026.Models.pharmacist;
using InfinityCoderzz_CMSV2026.Repositories.PharmacistRepositories;

namespace InfinityCoderzz_CMSV2026.Services.PharmacyServices
{
    public class MedicineDispensingServiceImpl : IMedicineDispensingService
    {
        private readonly IMedicineDispensingRepository _repository;

        public MedicineDispensingServiceImpl(IMedicineDispensingRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<Prescription> GetDispensablePrescriptions()
            => _repository.GetDispensablePrescriptions();

        public DispenseBillResult DispenseAndBill(int prescriptionId, int staffId, string? remarks)
            => _repository.DispenseAndBill(prescriptionId, staffId, remarks);

        public IEnumerable<DispensingHistoryViewModel> GetDispensingHistory()
            => _repository.GetDispensingHistory();

        public IEnumerable<MedicineDispensingItem> GetDispensingItems(int dispenseId)
            => _repository.GetDispensingItems(dispenseId);
    }
}
