namespace InfinityCoderzz_CMSV2026.Models.pharmacist
{
    public class MedicineDispensing
    {
        public int DispenseId { get; set; }

        public int PrescriptionId { get; set; }

        public int DispensedByStaffId { get; set; }

        public DateTime DispenseDate { get; set; }

        public string? Remarks { get; set; }
    }
}
