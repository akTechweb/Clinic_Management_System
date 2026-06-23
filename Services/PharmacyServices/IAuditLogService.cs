using InfinityCoderzz_CMSV2026.Models.pharmacist;

namespace InfinityCoderzz_CMSV2026.Services.PharmacyServices
{
    public interface IAuditLogService
    {
        IEnumerable<AuditLog> GetAuditLogs(DateTime? fromDate, DateTime? toDate);
        void AddAuditLog(int staffId, string action, string? remarks);
    }
}
