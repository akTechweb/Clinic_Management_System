namespace InfinityCoderzz_CMSV2026.Models.pharmacist
{
    public class DispensingHistoryViewModel
    {
        public int DispenseId { get; set; }
        public int PrescriptionId { get; set; }
        public string? PatientName { get; set; }
        public string? PharmacistName { get; set; }
        public DateTime DispenseDate { get; set; }
        public string? Remarks { get; set; }
        public int TotalItems { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
