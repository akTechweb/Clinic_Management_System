namespace InfinityCoderzz_CMSV2026.Models.pharmacist
{
   
   
        public class BillItemViewModel
        {
            public int BillItemId { get; set; }

            public int BillId { get; set; }

            public int MedicineId { get; set; }

            public string? MedicineName { get; set; }

            public int Quantity { get; set; }

            public decimal UnitPrice { get; set; }

            public decimal Amount { get; set; }
        }
    
}

