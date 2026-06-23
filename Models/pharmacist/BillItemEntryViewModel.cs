namespace InfinityCoderzz_CMSV2026.Models.pharmacist
{
   
        public class BillItemEntryViewModel
        {
            public int MedicineId { get; set; }

            public string? MedicineName { get; set; }

            public int Quantity { get; set; }

            public decimal UnitPrice { get; set; }

            public decimal Amount { get; set; }
        }
    
}

