using System.Collections.Generic;
using System.Threading.Tasks;
using InfinityCoderzz_CMSV2026.Models;

namespace InfinityCoderzz_CMSV2026.Repositories
{
    public interface ILabTechnicianRepository
    {
        Task<LabDashboardStatsViewModel> GetDashboardStatsAsync();
        Task<List<PendingLabRequestViewModel>> GetPendingLabRequestsAsync(string searchMMR);
        Task<List<LabPatientSearchViewModel>> SearchPatientByMMRAsync(string searchMMR);
        Task<EnterLabResultViewModel> GetLabRequestItemDetailsAsync(int requestItemId);
        Task<int> AddLabResultAsync(EnterLabResultViewModel model, int technicianId);
        Task<List<CompletedLabReportViewModel>> GetCompletedLabReportsAsync(string searchMMR);
        Task<LabResultPdfViewModel> GetLabResultDetailsAsync(int resultId);
        Task AddReportNotificationAsync(int resultId, string sentTo, string recipientType, string emailStatus);
        Task<List<UnbilledLabRequestViewModel>> GetUnbilledLabRequestsAsync(string searchMMR);
        Task<int> GenerateLabBillAsync(int requestId, int technicianId);
        Task<List<LabBillViewModel>> GetLabBillsAsync(string searchMMR);
        Task<LabBillDetailsViewModel> GetLabBillDetailsAsync(int billId);
        Task UpdateLabBillPaymentStatusAsync(int billId, string paymentStatus);
    }
}