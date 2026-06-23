namespace InfinityCoderzz_CMSV2026.Models.pharmacist
{
    public class BillPrescriptionLink
    {
        public int PrescriptionId { get; set; }
        public int DispenseId { get; set; }
        public string? PrescriptionStatus { get; set; }
    }
}
