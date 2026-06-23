using InfinityCoderzz_CMSV2026.Models.pharmacist;

namespace InfinityCoderzz_CMSV2026.Repositories.PharmacistRepositories
{
    public interface IPrescriptionRepository
    {
        IEnumerable<Prescription> GetAllPrescriptions();
        Prescription GetPrescriptionById(int prescriptionId);
        IEnumerable<PrescriptionItem> GetPrescriptionItems(int prescriptionId);
        void UpdatePrescriptionStatus(int prescriptionId, string status);
    }
}
