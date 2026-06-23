using InfinityCoderzz_CMSV2026.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InfinityCoderzzz_CMSV2026.Repositories
{
    public class BillRepository : IBillRepository
    {
        private readonly string _connectionString;

        public BillRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        #region Get All Bills

        public List<Bill> GetAllBills()
        {
            List<Bill> bills = new List<Bill>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand("sp_GetAllBills", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        bills.Add(MapBillHeader(reader));
                    }
                }
            }
            return bills;
        }

        #endregion

        #region Get Bill By ID

        public Bill GetBillById(int billId)
        {
            Bill bill = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand("sp_GetBillById", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@BillId", billId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    // First result set: bill header
                    if (reader.Read())
                    {
                        bill = MapBillHeader(reader);
                    }

                    if (bill == null) return null;

                    // Second result set: bill items
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            bill.Items.Add(new BillItem
                            {
                                BillItemId = Convert.ToInt32(reader["BillItemId"]),
                                BillId = Convert.ToInt32(reader["BillId"]),
                                ItemType = reader["ItemType"]?.ToString(),
                                ReferenceId = reader["ReferenceId"] != DBNull.Value ? Convert.ToInt32(reader["ReferenceId"]) : (int?)null,
                                Description = reader["Description"]?.ToString(),
                                Quantity = reader["Quantity"] != DBNull.Value ? Convert.ToInt32(reader["Quantity"]) : (int?)null,
                                UnitPrice = Convert.ToDecimal(reader["UnitPrice"]),
                                Amount = Convert.ToDecimal(reader["Amount"])
                            });
                        }
                    }

                    // Third result set: payments
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            bill.Payments.Add(new Payment
                            {
                                PaymentId = Convert.ToInt32(reader["PaymentId"]),
                                BillId = Convert.ToInt32(reader["BillId"]),
                                Amount = Convert.ToDecimal(reader["Amount"]),
                                PaymentMethod = reader["PaymentMethod"]?.ToString(),
                                TransactionReference = reader["TransactionReference"]?.ToString(),
                                PaymentDate = reader["PaymentDate"] != DBNull.Value ? Convert.ToDateTime(reader["PaymentDate"]) : (DateTime?)null,
                                PaymentStatus = reader["PaymentStatus"]?.ToString(),
                                Remarks = reader["Remarks"]?.ToString()
                            });
                        }
                    }
                }
            }
            return bill;
        }

        #endregion

        #region Generate Bill

        public void GenerateBill(int patientId, int appointmentId, out int billId, out decimal totalAmount, out string message)
        {
            int outBillId = 0;
            decimal outTotal = 0;
            string outMessage = string.Empty;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand("sp_GenerateBill", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@PatientId", patientId);
                cmd.Parameters.AddWithValue("@AppointmentId", appointmentId);

                SqlParameter outBillIdParam = new SqlParameter("@BillId", SqlDbType.Int) { Direction = ParameterDirection.Output };
                SqlParameter outTotalParam = new SqlParameter("@TotalAmount", SqlDbType.Decimal) { Precision = 12, Scale = 2, Direction = ParameterDirection.Output };
                SqlParameter outMsgParam = new SqlParameter("@Message", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };

                cmd.Parameters.Add(outBillIdParam);
                cmd.Parameters.Add(outTotalParam);
                cmd.Parameters.Add(outMsgParam);

                cmd.ExecuteNonQuery();

                outBillId = outBillIdParam.Value != DBNull.Value ? Convert.ToInt32(outBillIdParam.Value) : 0;
                outTotal = outTotalParam.Value != DBNull.Value ? Convert.ToDecimal(outTotalParam.Value) : 0;
                outMessage = outMsgParam.Value?.ToString();
            }

            billId = outBillId;
            totalAmount = outTotal;
            message = outMessage;
        }

        #endregion

        #region Process Payment

        public void ProcessPayment(Payment payment, out string message)
        {
            string outMessage = string.Empty;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand("sp_ProcessPayment", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@BillId", payment.BillId);
                cmd.Parameters.AddWithValue("@Amount", payment.Amount);
                cmd.Parameters.AddWithValue("@PaymentMethod", payment.PaymentMethod);
                cmd.Parameters.AddWithValue("@TransactionReference", (object)payment.TransactionReference ?? DBNull.Value);

                SqlParameter outPaymentId = new SqlParameter("@PaymentId", SqlDbType.Int) { Direction = ParameterDirection.Output };
                SqlParameter outMsg = new SqlParameter("@Message", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };

                cmd.Parameters.Add(outPaymentId);
                cmd.Parameters.Add(outMsg);

                cmd.ExecuteNonQuery();

                if (outPaymentId.Value != DBNull.Value)
                    payment.PaymentId = Convert.ToInt32(outPaymentId.Value);

                outMessage = outMsg.Value?.ToString();
            }

            message = outMessage;
        }

        #endregion

        #region Private Helper

        private Bill MapBillHeader(SqlDataReader reader)
        {
            return new Bill
            {
                BillId = Convert.ToInt32(reader["BillId"]),
                PatientId = Convert.ToInt32(reader["PatientId"]),
                AppointmentId = reader["AppointmentId"] != DBNull.Value ? Convert.ToInt32(reader["AppointmentId"]) : (int?)null,
                BillDate = reader["BillDate"] != DBNull.Value ? Convert.ToDateTime(reader["BillDate"]) : (DateTime?)null,
                TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                Status = reader["Status"]?.ToString(),
                Remarks = reader["Remarks"]?.ToString(),
                PatientName = reader["PatientName"]?.ToString(),
                PatientCode = reader["PatientCode"]?.ToString(),
                AppointmentNumber = reader["AppointmentNumber"]?.ToString(),
                DoctorName = reader["DoctorName"]?.ToString()
            };
        }

        #endregion
    }
}