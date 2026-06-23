using InfinityCoderzz_CMSV2026.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InfinityCoderzzz_CMSV2026.Repositories
{
    public class PatientVisitRepository : IPatientVisitRepository
    {
        private readonly string _connectionString;

        public PatientVisitRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        #region Get All Patient Visits

        public List<PatientVisit> GetAllPatientVisits()
        {
            List<PatientVisit> visits = new List<PatientVisit>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand("sp_GetAllPatientVisits", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        visits.Add(MapVisit(reader));
                    }
                }
            }
            return visits;
        }

        #endregion

        #region Get Visit By ID

        public PatientVisit GetPatientVisitById(int visitId)
        {
            PatientVisit visit = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand("sp_GetPatientVisitById", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@VisitId", visitId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        visit = MapVisit(reader);
                    }
                }
            }
            return visit;
        }

        #endregion

        #region Private Helper

        private PatientVisit MapVisit(SqlDataReader reader)
        {
            return new PatientVisit
            {
                VisitId = Convert.ToInt32(reader["VisitId"]),
                AppointmentId = Convert.ToInt32(reader["AppointmentId"]),
                PatientId = Convert.ToInt32(reader["PatientId"]),
                DoctorId = Convert.ToInt32(reader["DoctorId"]),
                VisitDate = reader["VisitDate"] != DBNull.Value ? Convert.ToDateTime(reader["VisitDate"]) : (DateTime?)null,
                VisitStatus = reader["VisitStatus"]?.ToString(),
                PatientName = reader["PatientName"]?.ToString(),
                DoctorName = reader["DoctorName"]?.ToString(),
                DepartmentName = reader["DepartmentName"]?.ToString(),
                AppointmentNumber = reader["AppointmentNumber"]?.ToString(),
                TokenNumber = reader["TokenNumber"] != DBNull.Value ? Convert.ToInt32(reader["TokenNumber"]) : 0
            };
        }

        #endregion
    }
}