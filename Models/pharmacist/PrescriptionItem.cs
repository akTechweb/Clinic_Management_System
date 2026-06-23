namespace InfinityCoderzz_CMSV2026.Models.pharmacist
{
   
        public class PrescriptionItem
        {
            public int PrescriptionItemId { get; set; }

            public int PrescriptionId { get; set; }

            public int MedicineId { get; set; }

            public string? MedicineName { get; set; }

            public string? Dosage { get; set; }

            public string? Frequency { get; set; }

            public string? Duration { get; set; }

            public int Quantity { get; set; }

            public string? Instructions { get; set; }
        }
    
}
