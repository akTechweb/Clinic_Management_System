namespace InfinityCoderzz_CMSV2026.Models
{
    public class DashboardStatsViewModel
    {
        public int TodayTotal { get; set; }
        public int TodayPending { get; set; }
        public int ConsultationsDone { get; set; }
    }

    public class LabDashboardStatsViewModel
    {
        public int TotalRequests { get; set; }
        public int PendingTests { get; set; }
        public int CompletedReports { get; set; }
        public int UnpaidBills { get; set; }
    }
}
