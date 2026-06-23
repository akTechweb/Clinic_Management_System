using InfinityCoderzz_CMSV2026.Controllers.Pharmacy;
using InfinityCoderzz_CMSV2026.Models.pharmacist;
using InfinityCoderzz_CMSV2026.Repositories.PharmacistRepositories;

namespace InfinityCoderzz_CMSV2026.Services.PharmacyServices
{
    public class BillServiceImpl : IPharmacyBillService
    {
        private readonly IPharmacyBillRepository _billRepository;

        public BillServiceImpl(IPharmacyBillRepository billRepository)
        {
            _billRepository = billRepository;
        }

        public IEnumerable<BillViewModel> GetAllBills()
            => _billRepository.GetAllBills();

        public IEnumerable<PatientLookup> GetPatients()
            => _billRepository.GetPatients();

        public IEnumerable<MedicineLookup> GetMedicinesForBilling()
            => _billRepository.GetMedicinesForBilling();

        public int CreateBill(CreateBillViewModel model, int staffId)
            => _billRepository.CreateBill(model, staffId);

        public BillViewModel GetBillById(int billId)
            => _billRepository.GetBillById(billId);

        public IEnumerable<BillItemViewModel> GetBillItems(int billId)
            => _billRepository.GetBillItems(billId);

        public BillPrescriptionLink? GetBillPrescriptionLink(int billId)
            => _billRepository.GetBillPrescriptionLink(billId);

        public void CancelBill(int billId, int staffId, string? reason)
            => _billRepository.CancelBill(billId, staffId, reason);
    }
}
