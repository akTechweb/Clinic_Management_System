namespace InfinityCoderzz_CMSV2026.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public int BillId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }   // Cash / Card / UPI
        public string TransactionReference { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string PaymentStatus { get; set; }   // Paid / Pending / Failed
        public string Remarks { get; set; }
    }
}
