namespace InfinityCoderzz_CMSV2026.Models
{
    public class Bill
    {
        public int BillId { get; set; }
        public int PatientId { get; set; }
        public int? AppointmentId { get; set; }
        public DateTime? BillDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }          // Draft / Paid / Pending
        public string Remarks { get; set; }

        // Display helpers
        public string PatientName { get; set; }
        public string PatientCode { get; set; }
        public string DoctorName { get; set; }
        public string AppointmentNumber { get; set; }
        public List<BillItem> Items { get; set; } = new List<BillItem>();
        public List<Payment> Payments { get; set; } = new List<Payment>();

    }
}
