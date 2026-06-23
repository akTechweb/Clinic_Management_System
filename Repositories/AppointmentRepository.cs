using InfinityCoderzz_CMSV2026.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InfinityCoderzzz_CMSV2026.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly string _connectionString;

        public AppointmentRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<Appointment> GetAllAppointments()
        {
            List<Appointment> appointments = new();

            using SqlConnection connection = new(_connectionString);
            connection.Open();

            using SqlCommand cmd = new("sp_GetAllAppointments", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                appointments.Add(MapAppointment(reader));
            }

            return appointments;
        }

        public Appointment GetAppointmentById(int appointmentId)
        {
            Appointment appointment = null;

            using SqlConnection connection = new(_connectionString);
            connection.Open();

            using SqlCommand cmd = new("sp_GetAppointmentById", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@AppointmentId", appointmentId);

            using SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                appointment = MapAppointment(reader);
            }

            return appointment;
        }

        public List<Appointment> GetAppointmentsByFilter(
            int? departmentId,
            int? doctorId,
            string patientCode,
            DateTime? fromDate,
            DateTime? toDate)
        {
            List<Appointment> appointments = new();

            using SqlConnection connection = new(_connectionString);
            connection.Open();

            using SqlCommand cmd = new("sp_GetAppointmentsByFilter", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@DepartmentId", (object?)departmentId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DoctorId", (object?)doctorId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PatientCode", string.IsNullOrWhiteSpace(patientCode) ? DBNull.Value : patientCode);
            cmd.Parameters.AddWithValue("@FromDate", (object?)fromDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ToDate", (object?)toDate ?? DBNull.Value);

            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                appointments.Add(MapAppointment(reader));
            }

            return appointments;
        }

        public void BookAppointment(Appointment appointment, out string message)
        {
            string outMessage = string.Empty;

            using SqlConnection connection = new(_connectionString);
            connection.Open();

            using SqlCommand cmd = new("sp_BookAppointment", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@PatientId", appointment.PatientId);
            cmd.Parameters.AddWithValue("@DoctorId", appointment.DoctorId);
            cmd.Parameters.AddWithValue("@AppointmentDate", appointment.AppointmentDate);
            cmd.Parameters.AddWithValue("@AppointmentTime", appointment.AppointmentTime);

            SqlParameter outAppointmentId = new("@AppointmentId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            SqlParameter outAppointmentNo = new("@AppointmentNo", SqlDbType.VarChar, 20)
            {
                Direction = ParameterDirection.Output
            };

            SqlParameter outMsg = new("@Message", SqlDbType.VarChar, 500)
            {
                Direction = ParameterDirection.Output
            };

            cmd.Parameters.Add(outAppointmentId);
            cmd.Parameters.Add(outAppointmentNo);
            cmd.Parameters.Add(outMsg);

            cmd.ExecuteNonQuery();

            if (outAppointmentId.Value != DBNull.Value)
                appointment.AppointmentId = Convert.ToInt32(outAppointmentId.Value);

            if (outAppointmentNo.Value != DBNull.Value)
                appointment.AppointmentNumber = outAppointmentNo.Value.ToString();

            outMessage = outMsg.Value?.ToString();
            message = outMessage;
        }

        public void CancelAppointment(int appointmentId, out string message)
        {
            string outMessage = string.Empty;

            using SqlConnection connection = new(_connectionString);
            connection.Open();

            using SqlCommand cmd = new("sp_CancelAppointment", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@AppointmentId", appointmentId);

            SqlParameter outMsg = new("@Message", SqlDbType.VarChar, 500)
            {
                Direction = ParameterDirection.Output
            };

            cmd.Parameters.Add(outMsg);
            cmd.ExecuteNonQuery();

            outMessage = outMsg.Value?.ToString();
            message = outMessage;
        }

        public List<Doctor> GetAllActiveDoctors()
        {
            List<Doctor> doctors = new();

            using SqlConnection connection = new(_connectionString);
            connection.Open();

            using SqlCommand cmd = new("sp_GetAllActiveDoctors", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                doctors.Add(new Doctor
                {
                    DoctorId = Convert.ToInt32(reader["DoctorId"]),
                    DepartmentId = reader["DepartmentId"] != DBNull.Value ? Convert.ToInt32(reader["DepartmentId"]) : 0,
                    FullName = reader["FullName"]?.ToString(),
                    DepartmentName = reader["DepartmentName"]?.ToString(),
                    ConsultationFee = reader["ConsultationFee"] != DBNull.Value ? Convert.ToDecimal(reader["ConsultationFee"]) : 0,
                    ExperienceYears = reader["ExperienceYears"] != DBNull.Value ? Convert.ToInt32(reader["ExperienceYears"]) : null
                });
            }

            return doctors;
        }

        private Appointment MapAppointment(SqlDataReader reader)
        {
            return new Appointment
            {
                AppointmentId = Convert.ToInt32(reader["AppointmentId"]),
                AppointmentNumber = reader["AppointmentNumber"]?.ToString(),
                PatientId = Convert.ToInt32(reader["PatientId"]),
                DoctorId = Convert.ToInt32(reader["DoctorId"]),
                AppointmentDate = Convert.ToDateTime(reader["AppointmentDate"]),
                AppointmentTime = (TimeSpan)reader["AppointmentTime"],
                TokenNumber = Convert.ToInt32(reader["TokenNumber"]),
                Status = reader["Status"]?.ToString(),
                CreatedAt = reader["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(reader["CreatedAt"]) : null,
                PatientName = reader["PatientName"]?.ToString(),
                PatientCode = reader["PatientCode"]?.ToString(),
                DoctorName = reader["DoctorName"]?.ToString(),
                DepartmentName = reader["DepartmentName"]?.ToString(),
                ConsultationFee = reader["ConsultationFee"] != DBNull.Value ? Convert.ToDecimal(reader["ConsultationFee"]) : 0
            };
        }
    }
}