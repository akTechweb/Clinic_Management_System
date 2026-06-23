using Microsoft.Data.SqlClient;
using System.Data;

namespace InfinityCoderzz_CMSV2026.Repositories.PharmacistRepositories
{
    public class PharmacyLoginRepositoryImpl : IPharmacyLoginRepository
    {
        private readonly string _connectionString;

        public PharmacyLoginRepositoryImpl(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConnStr");
        }

        public (int StaffId, string FullName) Login(string username, string passwordHash)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand("sp_PharmacistLogin", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Username", username);
            command.Parameters.AddWithValue("@PasswordHash", passwordHash);

            connection.Open();

            SqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                int staffId = Convert.ToInt32(reader["StaffId"]);
                string fullName = reader["FullName"].ToString();
                return (staffId, fullName);
            }

            return (0, string.Empty);
        }
    }
}
