namespace InfinityCoderzz_CMSV2026.Models.pharmacist
{
    public class MedicineDispensingItem
    {
        public int DispenseItemId { get; set; }

        public int DispenseId { get; set; }

        public int MedicineId { get; set; }

        public string? MedicineName { get; set; }

        public int QuantityDispensed { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal Amount { get; set; }
    }
}
