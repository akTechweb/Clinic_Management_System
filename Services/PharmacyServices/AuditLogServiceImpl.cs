using InfinityCoderzz_CMSV2026.Models.pharmacist;
using InfinityCoderzz_CMSV2026.Repositories.PharmacistRepositories;

namespace InfinityCoderzz_CMSV2026.Services.PharmacyServices
{
    public class AuditLogServiceImpl : IAuditLogService
    {
        private readonly IAuditLogRepository _repository;

        public AuditLogServiceImpl(IAuditLogRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<AuditLog> GetAuditLogs(DateTime? fromDate, DateTime? toDate)
            => _repository.GetAuditLogs(fromDate, toDate);

        public void AddAuditLog(int staffId, string action, string? remarks)
            => _repository.AddAuditLog(staffId, action, remarks);
    }
}
