using InfinityCoderzz_CMSV2026.Models.pharmacist;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InfinityCoderzz_CMSV2026.Repositories.PharmacistRepositories
{
    public class InventoryLogRepositoryImpl
        : IInventoryLogRepository
    {
        private readonly string _connectionString;

        public InventoryLogRepositoryImpl(
            IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("ConnStr");
        }

        public IEnumerable<MedicineInventoryLog>
            GetInventoryLogs()
        {
            List<MedicineInventoryLog> logs = new();

            using SqlConnection connection =
                new SqlConnection(_connectionString);

            using SqlCommand command =
                new SqlCommand(
                    "sp_GetInventoryLogs",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            connection.Open();

            SqlDataReader reader =
                command.ExecuteReader();

            while (reader.Read())
            {
                logs.Add(new MedicineInventoryLog
                {
                    InventoryLogId =
                        Convert.ToInt32(
                            reader["InventoryLogId"]),

                    MedicineName =
                        reader["MedicineName"]
                        .ToString(),

                    QuantityChanged =
                        Convert.ToInt32(
                            reader["QuantityChanged"]),

                    TransactionType =
                        reader["TransactionType"]
                        .ToString(),

                    TransactionDate =
                        Convert.ToDateTime(
                            reader["TransactionDate"]),

                    Remarks =
                        reader["Remarks"]
                        .ToString()
                });
            }

            return logs;
        }
    }
}
