using InfinityCoderzz_CMSV2026.Models.pharmacist;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InfinityCoderzz_CMSV2026.Repositories.PharmacistRepositories
{
    public class ReportRepositoryImpl : IReportRepository
    {
        private readonly string _connectionString;

        public ReportRepositoryImpl(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConnStr");
        }

        private SqlDataReader OpenReader(SqlConnection connection, string procedure,
            Action<SqlParameterCollection>? bind = null)
        {
            SqlCommand command = new(procedure, connection) { CommandType = CommandType.StoredProcedure };
            bind?.Invoke(command.Parameters);
            connection.Open();
            return command.ExecuteReader();
        }

        public IEnumerable<SalesSummaryRow> GetSalesSummary(DateTime? fromDate, DateTime? toDate)
        {
            List<SalesSummaryRow> list = new();
            using SqlConnection connection = new(_connectionString);
            using SqlDataReader reader = OpenReader(connection, "sp_ReportSalesSummary", p =>
            {
                p.AddWithValue("@FromDate", (object?)fromDate ?? DBNull.Value);
                p.AddWithValue("@ToDate", (object?)toDate ?? DBNull.Value);
            });
            while (reader.Read())
            {
                list.Add(new SalesSummaryRow
                {
                    SaleDate    = Convert.ToDateTime(reader["SaleDate"]),
                    BillCount   = Convert.ToInt32(reader["BillCount"]),
                    ItemsSold   = Convert.ToInt32(reader["ItemsSold"]),
                    TotalAmount = Convert.ToDecimal(reader["TotalAmount"])
                });
            }
            return list;
        }

        public IEnumerable<MedicineWiseSalesRow> GetMedicineWiseSales(DateTime? fromDate, DateTime? toDate)
        {
            List<MedicineWiseSalesRow> list = new();
            using SqlConnection connection = new(_connectionString);
            using SqlDataReader reader = OpenReader(connection, "sp_ReportMedicineWiseSales", p =>
            {
                p.AddWithValue("@FromDate", (object?)fromDate ?? DBNull.Value);
                p.AddWithValue("@ToDate", (object?)toDate ?? DBNull.Value);
            });
            while (reader.Read())
            {
                list.Add(new MedicineWiseSalesRow
                {
                    MedicineId   = reader["MedicineId"] == DBNull.Value ? 0 : Convert.ToInt32(reader["MedicineId"]),
                    MedicineName = reader["MedicineName"]?.ToString(),
                    QuantitySold = Convert.ToInt32(reader["QuantitySold"]),
                    TotalAmount  = Convert.ToDecimal(reader["TotalAmount"])
                });
            }
            return list;
        }

        public IEnumerable<StockStatusRow> GetStockStatus()
        {
            List<StockStatusRow> list = new();
            using SqlConnection connection = new(_connectionString);
            using SqlDataReader reader = OpenReader(connection, "sp_ReportStockStatus");
            while (reader.Read())
            {
                list.Add(new StockStatusRow
                {
                    MedicineId    = Convert.ToInt32(reader["MedicineId"]),
                    MedicineCode  = reader["MedicineCode"]?.ToString(),
                    MedicineName  = reader["MedicineName"]?.ToString(),
                    ReorderLevel  = Convert.ToInt32(reader["ReorderLevel"]),
                    TotalQuantity = Convert.ToInt32(reader["TotalQuantity"]),
                    StockStatus   = reader["StockStatus"]?.ToString()
                });
            }
            return list;
        }

        public IEnumerable<ExpiryReportRow> GetExpiryReport(int days)
        {
            List<ExpiryReportRow> list = new();
            using SqlConnection connection = new(_connectionString);
            using SqlDataReader reader = OpenReader(connection, "sp_ReportExpiry", p =>
                p.AddWithValue("@Days", days));
            while (reader.Read())
            {
                list.Add(new ExpiryReportRow
                {
                    StockId       = Convert.ToInt32(reader["StockId"]),
                    MedicineCode  = reader["MedicineCode"]?.ToString(),
                    MedicineName  = reader["MedicineName"]?.ToString(),
                    BatchNumber   = reader["BatchNumber"]?.ToString(),
                    Quantity      = Convert.ToInt32(reader["Quantity"]),
                    ExpiryDate    = Convert.ToDateTime(reader["ExpiryDate"]),
                    DaysRemaining = Convert.ToInt32(reader["DaysRemaining"]),
                    ExpiryStatus  = reader["ExpiryStatus"]?.ToString()
                });
            }
            return list;
        }

        public IEnumerable<LowStockReportRow> GetLowStockReport()
        {
            List<LowStockReportRow> list = new();
            using SqlConnection connection = new(_connectionString);
            using SqlDataReader reader = OpenReader(connection, "sp_ReportLowStock");
            while (reader.Read())
            {
                list.Add(new LowStockReportRow
                {
                    MedicineId    = Convert.ToInt32(reader["MedicineId"]),
                    MedicineCode  = reader["MedicineCode"]?.ToString(),
                    MedicineName  = reader["MedicineName"]?.ToString(),
                    ReorderLevel  = Convert.ToInt32(reader["ReorderLevel"]),
                    TotalQuantity = Convert.ToInt32(reader["TotalQuantity"])
                });
            }
            return list;
        }

        public IEnumerable<DispensingReportRow> GetDispensingReport(DateTime? fromDate, DateTime? toDate)
        {
            List<DispensingReportRow> list = new();
            using SqlConnection connection = new(_connectionString);
            using SqlDataReader reader = OpenReader(connection, "sp_ReportDispensing", p =>
            {
                p.AddWithValue("@FromDate", (object?)fromDate ?? DBNull.Value);
                p.AddWithValue("@ToDate", (object?)toDate ?? DBNull.Value);
            });
            while (reader.Read())
            {
                list.Add(new DispensingReportRow
                {
                    DispenseId     = Convert.ToInt32(reader["DispenseId"]),
                    PrescriptionId = Convert.ToInt32(reader["PrescriptionId"]),
                    PatientName    = reader["PatientName"]?.ToString(),
                    PharmacistName = reader["PharmacistName"]?.ToString(),
                    DispenseDate   = Convert.ToDateTime(reader["DispenseDate"]),
                    TotalItems     = Convert.ToInt32(reader["TotalItems"]),
                    TotalAmount    = Convert.ToDecimal(reader["TotalAmount"])
                });
            }
            return list;
        }
    }
}
