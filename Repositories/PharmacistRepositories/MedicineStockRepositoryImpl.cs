using InfinityCoderzz_CMSV2026.Models.pharmacist;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InfinityCoderzz_CMSV2026.Repositories.PharmacistRepositories
{
    public class MedicineStockRepositoryImpl : IMedicineStockRepository
    {
        private readonly string _connectionString;

        public MedicineStockRepositoryImpl(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("ConnStr");
        }

        #region Get All Medicine Stock

        public IEnumerable<MedicineStock> GetAllMedicineStock()
        {
            List<MedicineStock> stocks = new();

            using SqlConnection connection =
                new SqlConnection(_connectionString);

            using SqlCommand command =
                new SqlCommand("sp_GetAllMedicineStock", connection);

            command.CommandType = CommandType.StoredProcedure;

            connection.Open();

            SqlDataReader reader =
                command.ExecuteReader();

            while (reader.Read())
            {
                stocks.Add(new MedicineStock
                {
                    StockId = Convert.ToInt32(reader["StockId"]),
                    MedicineId = Convert.ToInt32(reader["MedicineId"]),
                    MedicineName = reader["MedicineName"].ToString(),
                    BatchNumber = reader["BatchNumber"].ToString(),
                    Quantity = Convert.ToInt32(reader["Quantity"]),
                    PurchasePrice = reader["PurchasePrice"] == DBNull.Value
                        ? null
                        : Convert.ToDecimal(reader["PurchasePrice"]),
                    ExpiryDate = Convert.ToDateTime(reader["ExpiryDate"]),
                    PurchaseDate = reader["PurchaseDate"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(reader["PurchaseDate"]),
                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                });
            }

            return stocks;
        }

        #endregion

        #region Get Stock By Id

        public MedicineStock GetMedicineStockById(int stockId)
        {
            MedicineStock stock = null;

            using SqlConnection connection =
                new SqlConnection(_connectionString);

            using SqlCommand command =
                new SqlCommand("sp_GetMedicineStockById", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue(
                "@StockId",
                stockId);

            connection.Open();

            SqlDataReader reader =
                command.ExecuteReader();

            if (reader.Read())
            {
                stock = new MedicineStock
                {
                    StockId = Convert.ToInt32(reader["StockId"]),
                    MedicineId = Convert.ToInt32(reader["MedicineId"]),
                    MedicineName = reader["MedicineName"].ToString(),
                    BatchNumber = reader["BatchNumber"].ToString(),
                    Quantity = Convert.ToInt32(reader["Quantity"]),
                    PurchasePrice = reader["PurchasePrice"] == DBNull.Value
                        ? null
                        : Convert.ToDecimal(reader["PurchasePrice"]),
                    ExpiryDate = Convert.ToDateTime(reader["ExpiryDate"]),
                    PurchaseDate = reader["PurchaseDate"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(reader["PurchaseDate"]),
                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                };
            }

            return stock;
        }

        #endregion


        #region Expiring Medicines

        public IEnumerable<MedicineStock>
            GetExpiringMedicines()
        {
            List<MedicineStock> stocks = new();

            using SqlConnection connection =
                new SqlConnection(_connectionString);

            using SqlCommand command =
                new SqlCommand(
                    "sp_GetExpiringMedicines",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            connection.Open();

            SqlDataReader reader =
                command.ExecuteReader();

            while (reader.Read())
            {
                stocks.Add(new MedicineStock
                {
                    StockId =
                        Convert.ToInt32(reader["StockId"]),

                    MedicineName =
                        reader["MedicineName"].ToString(),

                    BatchNumber =
                        reader["BatchNumber"].ToString(),

                    Quantity =
                        Convert.ToInt32(reader["Quantity"]),

                    ExpiryDate =
                        Convert.ToDateTime(reader["ExpiryDate"]),

                    DaysRemaining =
                        Convert.ToInt32(reader["DaysRemaining"])
                });
            }

            return stocks;
        }

        #endregion

        #region Add Stock

        public void AddMedicineStock(MedicineStock stock)
        {
            using SqlConnection connection =
                new SqlConnection(_connectionString);

            using SqlCommand command =
                new SqlCommand("sp_AddMedicineStock", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue(
                "@MedicineId",
                stock.MedicineId);

            command.Parameters.AddWithValue(
                "@BatchNumber",
                stock.BatchNumber);

            command.Parameters.AddWithValue(
                "@Quantity",
                stock.Quantity);

            command.Parameters.AddWithValue(
                "@PurchasePrice",
                stock.PurchasePrice ?? (object)DBNull.Value);

            command.Parameters.AddWithValue(
                "@ExpiryDate",
                stock.ExpiryDate);

            command.Parameters.AddWithValue(
                "@PurchaseDate",
                stock.PurchaseDate ?? (object)DBNull.Value);

            connection.Open();

            command.ExecuteNonQuery();

            using SqlCommand logCommand =
                new SqlCommand(
                    "sp_AddInventoryLog",
                    connection);

            logCommand.CommandType =
                CommandType.StoredProcedure;

            logCommand.Parameters.AddWithValue(
                "@MedicineId",
                stock.MedicineId);

            logCommand.Parameters.AddWithValue(
                "@QuantityChanged",
                stock.Quantity);

            logCommand.Parameters.AddWithValue(
                "@TransactionType",
                "Stock Added");

            logCommand.Parameters.AddWithValue(
                "@Remarks",
                "New stock batch added");

            logCommand.ExecuteNonQuery();
        }

        #endregion

        #region Update Stock

        public void UpdateMedicineStock(MedicineStock stock)
        {
            using SqlConnection connection =
                new SqlConnection(_connectionString);

            connection.Open();

            using SqlCommand command =
                new SqlCommand("sp_UpdateMedicineStock", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue(
                "@StockId",
                stock.StockId);

            command.Parameters.AddWithValue(
                "@Quantity",
                stock.Quantity);

            command.Parameters.AddWithValue(
                "@PurchasePrice",
                stock.PurchasePrice ?? (object)DBNull.Value);

            command.Parameters.AddWithValue(
                "@ExpiryDate",
                stock.ExpiryDate);

            command.ExecuteNonQuery();

            using SqlCommand logCommand =
                new SqlCommand(
                    "sp_AddInventoryLog",
                    connection);

            logCommand.CommandType =
                CommandType.StoredProcedure;

            logCommand.Parameters.AddWithValue(
                "@MedicineId",
                stock.MedicineId);

            logCommand.Parameters.AddWithValue(
                "@QuantityChanged",
                stock.Quantity);

            logCommand.Parameters.AddWithValue(
                "@TransactionType",
                "Stock Updated");

            logCommand.Parameters.AddWithValue(
                "@Remarks",
                "Stock details updated");

            logCommand.ExecuteNonQuery();
        }

        #endregion


        #region Expired Medicines

        public IEnumerable<MedicineStock>
            GetExpiredMedicines()
        {
            List<MedicineStock> stocks = new();

            using SqlConnection connection =
                new SqlConnection(_connectionString);

            using SqlCommand command =
                new SqlCommand(
                    "sp_GetExpiredMedicines",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            connection.Open();

            SqlDataReader reader =
                command.ExecuteReader();

            while (reader.Read())
            {
                stocks.Add(new MedicineStock
                {
                    StockId =
                        Convert.ToInt32(reader["StockId"]),

                    MedicineName =
                        reader["MedicineName"].ToString(),

                    BatchNumber =
                        reader["BatchNumber"].ToString(),

                    Quantity =
                        Convert.ToInt32(reader["Quantity"]),

                    ExpiryDate =
                        Convert.ToDateTime(reader["ExpiryDate"])
                });
            }

            return stocks;
        }

        #endregion



        #region Low Stock Medicines

        public IEnumerable<MedicineStock> GetLowStockMedicines()
        {
            List<MedicineStock> stocks = new();

            using SqlConnection connection =
                new SqlConnection(_connectionString);

            using SqlCommand command =
                new SqlCommand("sp_GetLowStockMedicines", connection);

            command.CommandType = CommandType.StoredProcedure;

            connection.Open();

            SqlDataReader reader =
                command.ExecuteReader();

            // Build a case-insensitive set of the columns the SP actually returned
            // so Batch/Expiry/etc. are mapped only when present (robust to older SPs).
            HashSet<string> columns =
                new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < reader.FieldCount; i++)
                columns.Add(reader.GetName(i));

            while (reader.Read())
            {
                MedicineStock item = new MedicineStock
                {
                    MedicineId = Convert.ToInt32(reader["MedicineId"]),
                    MedicineName = reader["MedicineName"].ToString(),
                    Quantity = Convert.ToInt32(reader["Quantity"])
                };

                if (columns.Contains("StockId") && reader["StockId"] != DBNull.Value)
                    item.StockId = Convert.ToInt32(reader["StockId"]);

                if (columns.Contains("BatchNumber") && reader["BatchNumber"] != DBNull.Value)
                    item.BatchNumber = reader["BatchNumber"].ToString();

                if (columns.Contains("ExpiryDate") && reader["ExpiryDate"] != DBNull.Value)
                    item.ExpiryDate = Convert.ToDateTime(reader["ExpiryDate"]);

                if (columns.Contains("DaysRemaining") && reader["DaysRemaining"] != DBNull.Value)
                    item.DaysRemaining = Convert.ToInt32(reader["DaysRemaining"]);

                stocks.Add(item);
            }

            return stocks;
        }

        #endregion

        #region Medicine Dropdown

        public IEnumerable<Medicine> GetAllMedicines()
        {
            List<Medicine> medicines = new();

            using SqlConnection connection =
                new SqlConnection(_connectionString);

            using SqlCommand command =
                new SqlCommand(
                    "sp_GetActiveMedicinesForDropdown",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            connection.Open();

            SqlDataReader reader =
                command.ExecuteReader();

            while (reader.Read())
            {
                medicines.Add(new Medicine
                {
                    MedicineId =
                        Convert.ToInt32(reader["MedicineId"]),

                    MedicineName =
                        reader["MedicineName"].ToString()
                });
            }

            return medicines;
        }

        #endregion

        #region Batch Exists

        public bool BatchExists(int medicineId, string batchNumber, int excludeStockId)
        {
            using SqlConnection connection =
                new SqlConnection(_connectionString);

            using SqlCommand command =
                new SqlCommand("sp_CheckStockBatchExists", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@MedicineId", medicineId);
            command.Parameters.AddWithValue("@BatchNumber", (object?)batchNumber ?? DBNull.Value);
            command.Parameters.AddWithValue("@ExcludeStockId", excludeStockId);

            connection.Open();

            object result = command.ExecuteScalar();

            return result != null && result != DBNull.Value && Convert.ToInt32(result) == 1;
        }

        #endregion
    }
}