namespace InfinityCoderzz_CMSV2026.Models.pharmacist
{
    
        public class MedicineInventoryLog
        {
            public int InventoryLogId { get; set; }

            public string? MedicineName { get; set; }

            public int QuantityChanged { get; set; }

            public string? TransactionType { get; set; }

            public DateTime TransactionDate { get; set; }

            public string? Remarks { get; set; }
        }
    
}
