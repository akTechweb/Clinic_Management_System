using InfinityCoderzz_CMSV2026.Models.pharmacist;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InfinityCoderzz_CMSV2026.Repositories.PharmacistRepositories
{
    public class PrescriptionRepositoryImpl : IPrescriptionRepository
    {
        private readonly string _connectionString;

        public PrescriptionRepositoryImpl(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConnStr");
        }

        public IEnumerable<Prescription> GetAllPrescriptions()
        {
            List<Prescription> prescriptions = new();

            using SqlConnection connection = new(_connectionString);
            using SqlCommand command = new("sp_GetAllPrescriptions", connection);
            command.CommandType = CommandType.StoredProcedure;

            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                prescriptions.Add(new Prescription
                {
                    PrescriptionId   = Convert.ToInt32(reader["PrescriptionId"]),
                    PatientId        = Convert.ToInt32(reader["PatientId"]),
                    PatientName      = reader["PatientName"].ToString(),
                    DoctorId         = Convert.ToInt32(reader["DoctorId"]),
                    DoctorName       = reader["DoctorName"].ToString(),
                    PrescriptionDate = Convert.ToDateTime(reader["PrescriptionDate"]),
                    Remarks          = reader["Remarks"]?.ToString(),
                    Status           = reader["Status"]?.ToString()
                });
            }

            return prescriptions;
        }

        public Prescription GetPrescriptionById(int prescriptionId)
        {
            Prescription prescription = null;

            using SqlConnection connection = new(_connectionString);
            using SqlCommand command = new("sp_GetPrescriptionById", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@PrescriptionId", prescriptionId);

            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                prescription = new Prescription
                {
                    PrescriptionId   = Convert.ToInt32(reader["PrescriptionId"]),
                    PatientId        = Convert.ToInt32(reader["PatientId"]),
                    PatientName      = reader["PatientName"].ToString(),
                    DoctorId         = Convert.ToInt32(reader["DoctorId"]),
                    DoctorName       = reader["DoctorName"].ToString(),
                    PrescriptionDate = Convert.ToDateTime(reader["PrescriptionDate"]),
                    Remarks          = reader["Remarks"]?.ToString(),
                    Status           = reader["Status"]?.ToString()
                };
            }

            return prescription;
        }

        public IEnumerable<PrescriptionItem> GetPrescriptionItems(int prescriptionId)
        {
            List<PrescriptionItem> items = new();

            using SqlConnection connection = new(_connectionString);
            using SqlCommand command = new("sp_GetPrescriptionItems", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@PrescriptionId", prescriptionId);

            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                items.Add(new PrescriptionItem
                {
                    PrescriptionItemId = Convert.ToInt32(reader["PrescriptionItemId"]),
                    PrescriptionId     = Convert.ToInt32(reader["PrescriptionId"]),
                    MedicineId         = Convert.ToInt32(reader["MedicineId"]),
                    MedicineName       = reader["MedicineName"].ToString(),
                    Dosage             = reader["Dosage"]?.ToString(),
                    Frequency          = reader["Frequency"]?.ToString(),
                    Duration           = reader["Duration"]?.ToString(),
                    Quantity           = Convert.ToInt32(reader["Quantity"]),
                    Instructions       = reader["Instructions"]?.ToString()
                });
            }

            return items;
        }

        public void UpdatePrescriptionStatus(int prescriptionId, string status)
        {
            using SqlConnection connection = new(_connectionString);
            using SqlCommand command = new("sp_UpdatePrescriptionStatus", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@PrescriptionId", prescriptionId);
            command.Parameters.AddWithValue("@Status", status);

            connection.Open();
            command.ExecuteNonQuery();
        }
    }
}
