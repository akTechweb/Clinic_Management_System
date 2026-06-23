namespace InfinityCoderzz_CMSV2026.Models.pharmacist
{
    public class PharmacyDashboard
    {
        public int TotalMedicines { get; set; }
        public int TotalStockBatches { get; set; }
        public int LowStockMedicines { get; set; }
        public int ExpiringMedicines { get; set; }
        public int ExpiredMedicines { get; set; }
        public int PendingPrescriptions { get; set; }
        public int TodaysBills { get; set; }
        public decimal TodaysRevenue { get; set; }
        public int AvailableMedicines { get; set; }
        public int ReorderRequired { get; set; }
        public int TodaysDispensed { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public List<MedicineStock> LowStockList { get; set; } = new();
        public List<MedicineStock> ExpiringList { get; set; } = new();
        public List<ChartPoint> RevenueChart { get; set; } = new();
        public List<ChartPoint> DispensingChart { get; set; } = new();
    }
}
