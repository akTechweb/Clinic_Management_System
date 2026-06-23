using System.Collections.Generic;

namespace InfinityCoderzz_CMSV2026.Models.pharmacist
{
    public class CreateBillViewModel
    {
        public int PatientId { get; set; }

        public int MedicineId { get; set; }

        public int Quantity { get; set; }

        public List<BillItemEntryViewModel>
            BillItems
        { get; set; }
            = new();

        public decimal TotalAmount { get; set; }
    }
}
