using InfinityCoderzz_CMSV2026.Models.pharmacist;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InfinityCoderzz_CMSV2026.Repositories.PharmacistRepositories
{
    public class AuditLogRepositoryImpl : IAuditLogRepository
    {
        private readonly string _connectionString;

        public AuditLogRepositoryImpl(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConnStr");
        }

        public IEnumerable<AuditLog> GetAuditLogs(DateTime? fromDate, DateTime? toDate)
        {
            List<AuditLog> list = new();

            using SqlConnection connection = new(_connectionString);
            using SqlCommand command = new("sp_GetAuditLogs", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@FromDate", (object?)fromDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@ToDate", (object?)toDate ?? DBNull.Value);

            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new AuditLog
                {
                    LogId    = Convert.ToInt32(reader["LogId"]),
                    UserId   = reader["UserId"] == DBNull.Value ? 0 : Convert.ToInt32(reader["UserId"]),
                    Username = reader["Username"]?.ToString(),
                    Action   = reader["Action"]?.ToString(),
                    Remarks  = reader["Remarks"]?.ToString(),
                    LogDate  = reader["LogDate"] == DBNull.Value
                                ? DateTime.Now
                                : Convert.ToDateTime(reader["LogDate"])
                });
            }

            return list;
        }

        public void AddAuditLog(int staffId, string action, string? remarks)
        {
            using SqlConnection connection = new(_connectionString);
            using SqlCommand command = new("sp_AddAuditLog", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@StaffId", staffId);
            command.Parameters.AddWithValue("@Action", action);
            command.Parameters.AddWithValue("@Remarks", (object?)remarks ?? DBNull.Value);

            connection.Open();
            command.ExecuteNonQuery();
        }
    }
}
