using InfinityCoderzz_CMSV2026.Models.pharmacist;

namespace InfinityCoderzz_CMSV2026.Repositories.PharmacistRepositories
{
    public interface IMedicineDispensingRepository
    {
        IEnumerable<Prescription> GetDispensablePrescriptions();
        DispenseBillResult DispenseAndBill(int prescriptionId, int staffId, string? remarks);
        IEnumerable<DispensingHistoryViewModel> GetDispensingHistory();
        IEnumerable<MedicineDispensingItem> GetDispensingItems(int dispenseId);
    }
}
