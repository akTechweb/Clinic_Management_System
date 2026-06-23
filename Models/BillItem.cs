namespace InfinityCoderzz_CMSV2026.Models
{
    public class BillItem
    {
        public int BillItemId { get; set; }
        public int BillId { get; set; }
        public string ItemType { get; set; }        // Consultation / Registration / Other
        public int? ReferenceId { get; set; }
        public string Description { get; set; }
        public int? Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; }
    }
}
