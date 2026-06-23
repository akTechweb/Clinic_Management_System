using InfinityCoderzz_CMSV2026.Models.pharmacist;

namespace InfinityCoderzz_CMSV2026.Services.PharmacyServices
{
    public interface IMedicineDispensingService
    {
        IEnumerable<Prescription> GetDispensablePrescriptions();
        DispenseBillResult DispenseAndBill(int prescriptionId, int staffId, string? remarks);
        IEnumerable<DispensingHistoryViewModel> GetDispensingHistory();
        IEnumerable<MedicineDispensingItem> GetDispensingItems(int dispenseId);
    }
}
