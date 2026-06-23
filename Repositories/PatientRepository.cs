using InfinityCoderzz_CMSV2026.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InfinityCoderzzz_CMSV2026.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly string _connectionString;

        public PatientRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<Patient> GetAllPatients()
        {
            List<Patient> patients = new();

            using SqlConnection connection = new(_connectionString);
            connection.Open();

            using SqlCommand cmd = new("sp_GetAllPatients", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                patients.Add(MapPatient(reader));
            }

            return patients;
        }

        public Patient GetPatientById(int patientId)
        {
            Patient patient = null;

            using SqlConnection connection = new(_connectionString);
            connection.Open();

            using SqlCommand cmd = new("sp_GetPatientById", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PatientId", patientId);

            using SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                patient = MapPatient(reader);
            }

            return patient;
        }

        public string GetNextPatientCode()
        {
            using SqlConnection connection = new(_connectionString);
            connection.Open();

            using SqlCommand cmd = new("sp_GetNextPatientCode", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            object result = cmd.ExecuteScalar();
            return result?.ToString() ?? "MMR000001";
        }

        public void RegisterPatient(Patient patient)
        {
            using SqlConnection connection = new(_connectionString);
            connection.Open();

            using SqlCommand cmd = new("sp_RegisterPatient", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@FullName", patient.FullName);
            cmd.Parameters.AddWithValue("@Gender", patient.Gender);
            cmd.Parameters.AddWithValue("@DOB", (object?)patient.DOB ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BloodGroup", (object?)patient.BloodGroup ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@MobileNumber", patient.MobileNumber);
            cmd.Parameters.AddWithValue("@Email", (object?)patient.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Address", (object?)patient.Address ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@EmergencyContactNumber", (object?)patient.EmergencyContactNumber ?? DBNull.Value);

            SqlParameter outPatientId = new("@PatientId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            SqlParameter outPatientCode = new("@PatientCode", SqlDbType.VarChar, 20)
            {
                Direction = ParameterDirection.Output
            };

            SqlParameter outMessage = new("@Message", SqlDbType.VarChar, 500)
            {
                Direction = ParameterDirection.Output
            };

            cmd.Parameters.Add(outPatientId);
            cmd.Parameters.Add(outPatientCode);
            cmd.Parameters.Add(outMessage);

            cmd.ExecuteNonQuery();

            string message = outMessage.Value?.ToString() ?? "";

            if (!string.IsNullOrWhiteSpace(message) &&
                message.StartsWith("ERROR"))
            {
                throw new Exception(message);
            }

            patient.PatientId = outPatientId.Value != DBNull.Value
                ? Convert.ToInt32(outPatientId.Value)
                : 0;

            patient.PatientCode = outPatientCode.Value?.ToString();

            if (patient.PatientId <= 0 || string.IsNullOrWhiteSpace(patient.PatientCode))
            {
                throw new Exception("Patient registration failed. Stored procedure did not return PatientId or PatientCode. SQL Message: " + message);
            }
        }

        public void UpdatePatient(Patient patient)
        {
            using SqlConnection connection = new(_connectionString);
            connection.Open();

            using SqlCommand cmd = new("sp_UpdatePatient", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@PatientId", patient.PatientId);
            cmd.Parameters.AddWithValue("@FullName", patient.FullName);
            cmd.Parameters.AddWithValue("@Gender", patient.Gender);
            cmd.Parameters.AddWithValue("@DOB", (object?)patient.DOB ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BloodGroup", (object?)patient.BloodGroup ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@MobileNumber", patient.MobileNumber);
            cmd.Parameters.AddWithValue("@Email", (object?)patient.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Address", (object?)patient.Address ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@EmergencyContactNumber", (object?)patient.EmergencyContactNumber ?? DBNull.Value);

            SqlParameter outMessage = new("@Message", SqlDbType.VarChar, 500)
            {
                Direction = ParameterDirection.Output
            };

            cmd.Parameters.Add(outMessage);
            cmd.ExecuteNonQuery();
        }

        public List<Patient> SearchPatients(string keyword)
        {
            List<Patient> patients = new();

            using SqlConnection connection = new(_connectionString);
            connection.Open();

            using SqlCommand cmd = new("sp_SearchPatients", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@SearchTerm", keyword ?? string.Empty);

            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                patients.Add(MapPatient(reader));
            }

            return patients;
        }

        private Patient MapPatient(SqlDataReader reader)
        {
            return new Patient
            {
                PatientId = Convert.ToInt32(reader["PatientId"]),
                PatientCode = reader["PatientCode"]?.ToString(),
                FullName = reader["FullName"]?.ToString(),
                Gender = reader["Gender"]?.ToString(),
                DOB = reader["DOB"] != DBNull.Value ? Convert.ToDateTime(reader["DOB"]) : null,
                BloodGroup = reader["BloodGroup"]?.ToString(),
                MobileNumber = reader["MobileNumber"]?.ToString(),
                Email = reader["Email"]?.ToString(),
                Address = reader["Address"]?.ToString(),
                EmergencyContactNumber = reader["EmergencyContactNumber"]?.ToString(),
                RegistrationDate = reader["RegistrationDate"] != DBNull.Value ? Convert.ToDateTime(reader["RegistrationDate"]) : null,
                IsActive = reader["IsActive"] != DBNull.Value && Convert.ToBoolean(reader["IsActive"])
            };
        }
    }
}