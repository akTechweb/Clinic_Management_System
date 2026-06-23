namespace InfinityCoderzz_CMSV2026.Models.pharmacist
{
    public class AuditLog
    {
        public int LogId { get; set; }
        public int UserId { get; set; }
        public string? Username { get; set; }
        public string? Action { get; set; }
        public string? Remarks { get; set; }
        public DateTime LogDate { get; set; }
    }
}
