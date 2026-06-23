using InfinityCoderzz_CMSV2026.Controllers.Pharmacy;
using InfinityCoderzz_CMSV2026.Models.pharmacist;

namespace InfinityCoderzz_CMSV2026.Services.PharmacyServices
{
    public interface IPharmacyBillService
    {
        IEnumerable<BillViewModel> GetAllBills();
        IEnumerable<PatientLookup> GetPatients();
        IEnumerable<MedicineLookup> GetMedicinesForBilling();
        int CreateBill(CreateBillViewModel model, int staffId);
        BillViewModel GetBillById(int billId);
        IEnumerable<BillItemViewModel> GetBillItems(int billId);
        BillPrescriptionLink? GetBillPrescriptionLink(int billId);
        void CancelBill(int billId, int staffId, string? reason);
    }
}
