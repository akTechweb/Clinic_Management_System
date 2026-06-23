using InfinityCoderzz_CMSV2026.Controllers.Pharmacy;
using InfinityCoderzz_CMSV2026.Models.pharmacist;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InfinityCoderzz_CMSV2026.Repositories.PharmacistRepositories
{
    public class BillRepositoryImpl : IPharmacyBillRepository
    {
        private readonly string _connectionString;

        public BillRepositoryImpl(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConnStr");
        }

        #region Get Patients

        public IEnumerable<PatientLookup> GetPatients()
        {
            List<PatientLookup> patients = new();
            using SqlConnection conn = new(_connectionString);
            using SqlCommand cmd = new("sp_GetPatients", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                patients.Add(new PatientLookup
                {
                    PatientId   = Convert.ToInt32(reader["PatientId"]),
                    PatientCode = reader["PatientCode"].ToString(),
                    FullName    = reader["FullName"].ToString()
                });
            }
            return patients;
        }

        #endregion

        #region Get Medicines For Billing

        public IEnumerable<MedicineLookup> GetMedicinesForBilling()
        {
            List<MedicineLookup> medicines = new();
            using SqlConnection conn = new(_connectionString);
            using SqlCommand cmd = new("sp_GetMedicinesForBilling", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                medicines.Add(new MedicineLookup
                {
                    MedicineId   = Convert.ToInt32(reader["MedicineId"]),
                    MedicineCode = reader["MedicineCode"].ToString(),
                    MedicineName = reader["MedicineName"].ToString(),
                    UnitPrice    = Convert.ToDecimal(reader["UnitPrice"])
                });
            }
            return medicines;
        }

        #endregion

        #region Get All Bills

        public IEnumerable<BillViewModel> GetAllBills()
        {
            List<BillViewModel> bills = new();
            using SqlConnection conn = new(_connectionString);
            using SqlCommand cmd = new("sp_GetPharmacyBills", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                bills.Add(new BillViewModel
                {
                    BillId      = Convert.ToInt32(reader["BillId"]),
                    PatientId   = Convert.ToInt32(reader["PatientId"]),
                    BillDate    = Convert.ToDateTime(reader["BillDate"]),
                    TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                    Status      = reader["Status"].ToString(),
                    PatientName = reader["PatientName"].ToString(),
                    PatientCode = reader["PatientCode"].ToString()
                });
            }
            return bills;
        }

        #endregion

        #region Get Bill By Id

        public BillViewModel GetBillById(int billId)
        {
            BillViewModel bill = null;
            using SqlConnection conn = new(_connectionString);
            using SqlCommand cmd = new("sp_GetPharmacyBillById", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BillId", billId);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                bill = new BillViewModel
                {
                    BillId      = Convert.ToInt32(reader["BillId"]),
                    PatientId   = Convert.ToInt32(reader["PatientId"]),
                    BillDate    = Convert.ToDateTime(reader["BillDate"]),
                    TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                    Status      = reader["Status"].ToString(),
                    PatientName = reader["PatientName"].ToString(),
                    PatientCode = reader["PatientCode"].ToString()
                };
            }
            return bill;
        }

        #endregion

        #region Get Bill Items

        public IEnumerable<BillItemViewModel> GetBillItems(int billId)
        {
            List<BillItemViewModel> items = new();
            using SqlConnection conn = new(_connectionString);
            using SqlCommand cmd = new("sp_GetPharmacyBillItemsByBill", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BillId", billId);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                items.Add(new BillItemViewModel
                {
                    BillItemId   = Convert.ToInt32(reader["BillItemId"]),
                    BillId       = Convert.ToInt32(reader["BillId"]),
                    MedicineId   = Convert.ToInt32(reader["MedicineId"]),
                    MedicineName = reader["MedicineName"].ToString(),
                    Quantity     = Convert.ToInt32(reader["Quantity"]),
                    UnitPrice    = Convert.ToDecimal(reader["UnitPrice"]),
                    Amount       = Convert.ToDecimal(reader["Amount"])
                });
            }
            return items;
        }

        #endregion

        #region Get Bill -> Prescription Link

        public BillPrescriptionLink? GetBillPrescriptionLink(int billId)
        {
            using SqlConnection conn = new(_connectionString);
            using SqlCommand cmd = new("sp_GetPrescriptionLinkByBill", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BillId", billId);
            conn.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new BillPrescriptionLink
                {
                    PrescriptionId     = Convert.ToInt32(reader["PrescriptionId"]),
                    DispenseId         = Convert.ToInt32(reader["DispenseId"]),
                    PrescriptionStatus = reader["PrescriptionStatus"]?.ToString()
                };
            }
            return null;
        }

        #endregion

        #region Create Bill

        public int CreateBill(CreateBillViewModel model, int staffId)
        {
            using SqlConnection conn = new(_connectionString);
            conn.Open();
            using SqlTransaction tx = conn.BeginTransaction();
            try
            {
                SqlCommand billCmd = new("sp_CreatePharmacyBillHeader", conn, tx);
                billCmd.CommandType = CommandType.StoredProcedure;
                billCmd.Parameters.AddWithValue("@PatientId", model.PatientId);
                billCmd.Parameters.AddWithValue("@TotalAmount", model.TotalAmount);
                billCmd.Parameters.AddWithValue("@GeneratedByStaffId", staffId);

                SqlParameter billIdParam = new("@BillId", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                billCmd.Parameters.Add(billIdParam);
                billCmd.ExecuteNonQuery();

                int billId = Convert.ToInt32(billIdParam.Value);

                foreach (var item in model.BillItems)
                {
                    SqlCommand itemCmd = new("sp_AddPharmacyBillItem", conn, tx);
                    itemCmd.CommandType = CommandType.StoredProcedure;
                    itemCmd.Parameters.AddWithValue("@BillId", billId);
                    itemCmd.Parameters.AddWithValue("@MedicineId", item.MedicineId);
                    itemCmd.Parameters.AddWithValue("@MedicineName", item.MedicineName ?? string.Empty);
                    itemCmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                    itemCmd.Parameters.AddWithValue("@UnitPrice", item.UnitPrice);
                    itemCmd.Parameters.AddWithValue("@Amount", item.Amount);
                    itemCmd.ExecuteNonQuery();
                }

                tx.Commit();
                return billId;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        #endregion

        #region Cancel Bill

        public void CancelBill(int billId, int staffId, string? reason)
        {
            using SqlConnection conn = new(_connectionString);
            using SqlCommand cmd = new("sp_CancelPharmacyBill", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BillId", billId);
            cmd.Parameters.AddWithValue("@StaffId", staffId);
            cmd.Parameters.AddWithValue("@Reason", (object?)reason ?? DBNull.Value);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        #endregion
    }
}
