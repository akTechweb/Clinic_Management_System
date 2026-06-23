using InfinityCoderzz_CMSV2026.Models.pharmacist;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InfinityCoderzz_CMSV2026.Repositories.PharmacistRepositories
{
    public class PharmacyDashboardRepositoryImpl : IPharmacyDashboardRepository
    {
        private readonly string _connectionString;

        public PharmacyDashboardRepositoryImpl(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConnStr");
        }

        public PharmacyDashboard GetDashboardData()
        {
            PharmacyDashboard dashboard = new();

            // --- Core counts via existing SP ---
            try
            {
                using SqlConnection conn = new(_connectionString);
                using SqlCommand cmd = new("sp_GetPharmacyDashboard", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    dashboard.TotalMedicines    = Convert.ToInt32(reader["TotalMedicines"]);
                    dashboard.TotalStockBatches = Convert.ToInt32(reader["TotalStockBatches"]);
                    dashboard.LowStockMedicines = Convert.ToInt32(reader["LowStockMedicines"]);
                    dashboard.ExpiringMedicines = Convert.ToInt32(reader["ExpiringMedicines"]);
                    dashboard.ExpiredMedicines  = Convert.ToInt32(reader["ExpiredMedicines"]);

                    if (reader.HasRows && HasColumn(reader, "PendingPrescriptions"))
                        dashboard.PendingPrescriptions = Convert.ToInt32(reader["PendingPrescriptions"]);

                    if (reader.HasRows && HasColumn(reader, "TodaysBills"))
                        dashboard.TodaysBills = Convert.ToInt32(reader["TodaysBills"]);

                    if (reader.HasRows && HasColumn(reader, "TodaysRevenue"))
                        dashboard.TodaysRevenue = reader["TodaysRevenue"] == DBNull.Value
                            ? 0m : Convert.ToDecimal(reader["TodaysRevenue"]);

                    if (HasColumn(reader, "AvailableMedicines"))
                        dashboard.AvailableMedicines = Convert.ToInt32(reader["AvailableMedicines"]);

                    if (HasColumn(reader, "ReorderRequired"))
                        dashboard.ReorderRequired = Convert.ToInt32(reader["ReorderRequired"]);

                    if (HasColumn(reader, "TodaysDispensed"))
                        dashboard.TodaysDispensed = Convert.ToInt32(reader["TodaysDispensed"]);

                    if (HasColumn(reader, "MonthlyRevenue"))
                        dashboard.MonthlyRevenue = reader["MonthlyRevenue"] == DBNull.Value
                            ? 0m : Convert.ToDecimal(reader["MonthlyRevenue"]);
                }
            }
            catch { /* SP not yet created — dashboard shows zeros */ }

            // --- Charts ---
            dashboard.RevenueChart    = LoadChart("sp_GetPharmacyRevenueChart");
            dashboard.DispensingChart = LoadChart("sp_GetPharmacyDispensingChart");

            // --- Low stock list ---
            try
            {
                using SqlConnection conn2 = new(_connectionString);
                using SqlCommand cmd2 = new("sp_GetLowStockList", conn2);
                cmd2.CommandType = CommandType.StoredProcedure;
                conn2.Open();
                SqlDataReader r2 = cmd2.ExecuteReader();
                while (r2.Read())
                {
                    dashboard.LowStockList.Add(MapMedicineStock(r2));
                }
            }
            catch { /* SP not yet available */ }

            // --- Expiring list ---
            try
            {
                using SqlConnection conn3 = new(_connectionString);
                using SqlCommand cmd3 = new("sp_GetExpiringList", conn3);
                cmd3.CommandType = CommandType.StoredProcedure;
                conn3.Open();
                SqlDataReader r3 = cmd3.ExecuteReader();
                while (r3.Read())
                {
                    dashboard.ExpiringList.Add(MapMedicineStock(r3));
                }
            }
            catch { /* SP not yet available */ }

            return dashboard;
        }

        private List<ChartPoint> LoadChart(string procedure)
        {
            List<ChartPoint> points = new();
            try
            {
                using SqlConnection conn = new(_connectionString);
                using SqlCommand cmd = new(procedure, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    points.Add(new ChartPoint
                    {
                        Label = reader["Label"]?.ToString() ?? string.Empty,
                        Value = reader["Value"] == DBNull.Value ? 0m : Convert.ToDecimal(reader["Value"])
                    });
                }
            }
            catch { /* SP not yet available */ }
            return points;
        }

        private static MedicineStock MapMedicineStock(SqlDataReader reader)
        {
            return new MedicineStock
            {
                StockId      = Convert.ToInt32(reader["StockId"]),
                MedicineId   = Convert.ToInt32(reader["MedicineId"]),
                MedicineName = reader["MedicineName"]?.ToString(),
                BatchNumber  = reader["BatchNumber"]?.ToString() ?? string.Empty,
                Quantity     = Convert.ToInt32(reader["Quantity"]),
                ExpiryDate   = Convert.ToDateTime(reader["ExpiryDate"]),
                DaysRemaining = reader["DaysRemaining"] == DBNull.Value
                    ? 0 : Convert.ToInt32(reader["DaysRemaining"])
            };
        }

        private static bool HasColumn(SqlDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
                if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                    return true;
            return false;
        }
    }
}
