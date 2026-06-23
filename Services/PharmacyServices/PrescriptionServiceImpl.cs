using InfinityCoderzz_CMSV2026.Models.pharmacist;
using InfinityCoderzz_CMSV2026.Repositories.PharmacistRepositories;

namespace InfinityCoderzz_CMSV2026.Services.PharmacyServices
{
    public class PrescriptionServiceImpl : IPrescriptionService
    {
        private readonly IPrescriptionRepository _prescriptionRepository;

        public PrescriptionServiceImpl(IPrescriptionRepository prescriptionRepository)
        {
            _prescriptionRepository = prescriptionRepository;
        }

        public IEnumerable<Prescription> GetAllPrescriptions()
            => _prescriptionRepository.GetAllPrescriptions();

        public Prescription GetPrescriptionById(int prescriptionId)
            => _prescriptionRepository.GetPrescriptionById(prescriptionId);

        public IEnumerable<PrescriptionItem> GetPrescriptionItems(int prescriptionId)
            => _prescriptionRepository.GetPrescriptionItems(prescriptionId);

        public void UpdatePrescriptionStatus(int prescriptionId, string status)
            => _prescriptionRepository.UpdatePrescriptionStatus(prescriptionId, status);
    }
}
