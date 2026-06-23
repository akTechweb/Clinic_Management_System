using InfinityCoderzz_CMSV2026.Models.pharmacist;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InfinityCoderzz_CMSV2026.Repositories.PharmacistRepositories
{
    public class MedicineDispensingRepositoryImpl : IMedicineDispensingRepository
    {
        private readonly string _connectionString;

        public MedicineDispensingRepositoryImpl(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConnStr");
        }

        // ---- Prescriptions that can still be dispensed -------------------
        public IEnumerable<Prescription> GetDispensablePrescriptions()
        {
            List<Prescription> list = new();

            using SqlConnection connection = new(_connectionString);
            using SqlCommand command = new("sp_GetAllPrescriptions", connection);
            command.CommandType = CommandType.StoredProcedure;

            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                string? status = reader["Status"]?.ToString();
                if (!string.IsNullOrEmpty(status) &&
                    (status.Equals("Dispensed", StringComparison.OrdinalIgnoreCase) ||
                     status.Equals("Cancelled", StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }

                list.Add(new Prescription
                {
                    PrescriptionId   = Convert.ToInt32(reader["PrescriptionId"]),
                    PatientId        = reader["PatientId"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PatientId"]),
                    PatientName      = reader["PatientName"]?.ToString(),
                    DoctorId         = reader["DoctorId"] == DBNull.Value ? 0 : Convert.ToInt32(reader["DoctorId"]),
                    DoctorName       = reader["DoctorName"]?.ToString(),
                    PrescriptionDate = reader["PrescriptionDate"] == DBNull.Value
                                        ? DateTime.Now
                                        : Convert.ToDateTime(reader["PrescriptionDate"]),
                    Remarks          = reader["Remarks"]?.ToString(),
                    Status           = string.IsNullOrEmpty(status) ? "Pending" : status
                });
            }

            return list;
        }

        // ---- Dispense an entire prescription AND auto-generate a linked bill --
        // Stock is deducted ONCE (inside sp_DispenseMedicine). The bill lines are
        // inserted with sp_AddPharmacyBillItemNoStock (NO second deduction), then
        // the bill is linked to the prescription/dispense via the link table.
        // All steps run in a single transaction — any failure rolls everything back.
        public DispenseBillResult DispenseAndBill(int prescriptionId, int staffId, string? remarks)
        {
            List<(int MedicineId, int Quantity)> items = GetItemsToDispense(prescriptionId);
            if (items.Count == 0)
                throw new InvalidOperationException("This prescription has no medicines to dispense.");

            int patientId = GetPrescriptionPatientId(prescriptionId);
            if (patientId <= 0)
                throw new InvalidOperationException("Could not resolve the patient for this prescription.");

            using SqlConnection connection = new(_connectionString);
            connection.Open();
            using SqlTransaction transaction = connection.BeginTransaction();

            try
            {
                // 1. dispensing header  -> returns DispenseId
                int dispenseId;
                using (SqlCommand createCmd = new("sp_CreateMedicineDispensing", connection, transaction))
                {
                    createCmd.CommandType = CommandType.StoredProcedure;
                    createCmd.Parameters.AddWithValue("@PrescriptionId", prescriptionId);
                    createCmd.Parameters.AddWithValue("@DispensedByStaffId", staffId);
                    createCmd.Parameters.AddWithValue("@Remarks", (object?)remarks ?? DBNull.Value);

                    object? scalar = createCmd.ExecuteScalar();
                    dispenseId = scalar == null ? 0 : Convert.ToInt32(scalar);
                }

                if (dispenseId <= 0)
                    throw new InvalidOperationException("Failed to create the dispensing record.");

                // 2. dispense each item (FEFO deduction + inventory log inside the SP)
                foreach (var (medicineId, quantity) in items)
                {
                    using SqlCommand dispCmd = new("sp_DispenseMedicine", connection, transaction);
                    dispCmd.CommandType = CommandType.StoredProcedure;
                    dispCmd.Parameters.AddWithValue("@DispenseId", dispenseId);
                    dispCmd.Parameters.AddWithValue("@MedicineId", medicineId);
                    dispCmd.Parameters.AddWithValue("@QuantityDispensed", quantity);
                    dispCmd.ExecuteNonQuery();
                }

                // 3. read the priced dispensing lines back (same transaction) so the
                //    bill mirrors exactly what was dispensed (qty, unit price, amount)
                List<(int MedicineId, string MedicineName, int Quantity, decimal UnitPrice, decimal Amount)> billLines = new();
                decimal total = 0m;
                using (SqlCommand readCmd = new("sp_GetDispensingItems", connection, transaction))
                {
                    readCmd.CommandType = CommandType.StoredProcedure;
                    readCmd.Parameters.AddWithValue("@DispenseId", dispenseId);
                    using SqlDataReader reader = readCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        int medId      = Convert.ToInt32(reader["MedicineId"]);
                        string medName = reader["MedicineName"]?.ToString() ?? string.Empty;
                        int qty        = Convert.ToInt32(reader["QuantityDispensed"]);
                        decimal price  = Convert.ToDecimal(reader["UnitPrice"]);
                        decimal amount = Convert.ToDecimal(reader["Amount"]);
                        billLines.Add((medId, medName, qty, price, amount));
                        total += amount;
                    }
                }

                // 4. bill header -> returns BillId (the SP also writes its own audit log)
                int billId;
                using (SqlCommand billCmd = new("sp_CreatePharmacyBillHeader", connection, transaction))
                {
                    billCmd.CommandType = CommandType.StoredProcedure;
                    billCmd.Parameters.AddWithValue("@PatientId", patientId);
                    billCmd.Parameters.AddWithValue("@TotalAmount", total);
                    billCmd.Parameters.AddWithValue("@GeneratedByStaffId", staffId);
                    SqlParameter billIdParam = new("@BillId", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    billCmd.Parameters.Add(billIdParam);
                    billCmd.ExecuteNonQuery();
                    billId = Convert.ToInt32(billIdParam.Value);
                }

                // 5. bill items WITHOUT a second stock deduction
                foreach (var line in billLines)
                {
                    using SqlCommand itemCmd = new("sp_AddPharmacyBillItemNoStock", connection, transaction);
                    itemCmd.CommandType = CommandType.StoredProcedure;
                    itemCmd.Parameters.AddWithValue("@BillId", billId);
                    itemCmd.Parameters.AddWithValue("@MedicineId", line.MedicineId);
                    itemCmd.Parameters.AddWithValue("@MedicineName", line.MedicineName);
                    itemCmd.Parameters.AddWithValue("@Quantity", line.Quantity);
                    itemCmd.Parameters.AddWithValue("@UnitPrice", line.UnitPrice);
                    itemCmd.Parameters.AddWithValue("@Amount", line.Amount);
                    itemCmd.ExecuteNonQuery();
                }

                // 6. link bill <-> prescription <-> dispense
                using (SqlCommand linkCmd = new("sp_LinkBillToPrescription", connection, transaction))
                {
                    linkCmd.CommandType = CommandType.StoredProcedure;
                    linkCmd.Parameters.AddWithValue("@BillId", billId);
                    linkCmd.Parameters.AddWithValue("@PrescriptionId", prescriptionId);
                    linkCmd.Parameters.AddWithValue("@DispenseId", dispenseId);
                    linkCmd.ExecuteNonQuery();
                }

                // 7. mark prescription as dispensed
                using (SqlCommand statusCmd = new("sp_UpdatePrescriptionStatus", connection, transaction))
                {
                    statusCmd.CommandType = CommandType.StoredProcedure;
                    statusCmd.Parameters.AddWithValue("@PrescriptionId", prescriptionId);
                    statusCmd.Parameters.AddWithValue("@Status", "Dispensed");
                    statusCmd.ExecuteNonQuery();
                }

                // 8. audit log
                using (SqlCommand auditCmd = new("sp_AddAuditLog", connection, transaction))
                {
                    auditCmd.CommandType = CommandType.StoredProcedure;
                    auditCmd.Parameters.AddWithValue("@StaffId", staffId);
                    auditCmd.Parameters.AddWithValue("@Action", "Dispense & Bill");
                    auditCmd.Parameters.AddWithValue("@Remarks",
                        $"Dispensed prescription #{prescriptionId} ({billLines.Count} item(s)) and generated bill #{billId}.");
                    auditCmd.ExecuteNonQuery();
                }

                transaction.Commit();
                return new DispenseBillResult { DispenseId = dispenseId, BillId = billId };
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        private int GetPrescriptionPatientId(int prescriptionId)
        {
            using SqlConnection connection = new(_connectionString);
            using SqlCommand command = new("sp_GetPrescriptionById", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@PrescriptionId", prescriptionId);

            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
                return reader["PatientId"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PatientId"]);
            return 0;
        }

        private List<(int MedicineId, int Quantity)> GetItemsToDispense(int prescriptionId)
        {
            List<(int, int)> items = new();

            using SqlConnection connection = new(_connectionString);
            using SqlCommand command = new("sp_GetPrescriptionItems", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@PrescriptionId", prescriptionId);

            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                int medicineId = Convert.ToInt32(reader["MedicineId"]);
                int quantity   = reader["Quantity"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Quantity"]);
                if (quantity > 0)
                    items.Add((medicineId, quantity));
            }

            return items;
        }

        // ---- History -----------------------------------------------------
        public IEnumerable<DispensingHistoryViewModel> GetDispensingHistory()
        {
            List<DispensingHistoryViewModel> list = new();

            using SqlConnection connection = new(_connectionString);
            using SqlCommand command = new("sp_GetDispensingHistory", connection);
            command.CommandType = CommandType.StoredProcedure;

            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new DispensingHistoryViewModel
                {
                    DispenseId     = Convert.ToInt32(reader["DispenseId"]),
                    PrescriptionId = Convert.ToInt32(reader["PrescriptionId"]),
                    PatientName    = reader["PatientName"]?.ToString(),
                    PharmacistName = reader["PharmacistName"]?.ToString(),
                    DispenseDate   = Convert.ToDateTime(reader["DispenseDate"]),
                    Remarks        = reader["Remarks"]?.ToString(),
                    TotalItems     = Convert.ToInt32(reader["TotalItems"]),
                    TotalAmount    = reader["TotalAmount"] == DBNull.Value
                                        ? 0m
                                        : Convert.ToDecimal(reader["TotalAmount"])
                });
            }

            return list;
        }

        // ---- Items for one dispensing ------------------------------------
        public IEnumerable<MedicineDispensingItem> GetDispensingItems(int dispenseId)
        {
            List<MedicineDispensingItem> items = new();

            using SqlConnection connection = new(_connectionString);
            using SqlCommand command = new("sp_GetDispensingItems", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@DispenseId", dispenseId);

            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                items.Add(new MedicineDispensingItem
                {
                    DispenseItemId    = Convert.ToInt32(reader["DispenseItemId"]),
                    DispenseId        = Convert.ToInt32(reader["DispenseId"]),
                    MedicineId        = Convert.ToInt32(reader["MedicineId"]),
                    MedicineName      = reader["MedicineName"]?.ToString(),
                    QuantityDispensed = Convert.ToInt32(reader["QuantityDispensed"]),
                    UnitPrice         = Convert.ToDecimal(reader["UnitPrice"]),
                    Amount            = Convert.ToDecimal(reader["Amount"])
                });
            }

            return items;
        }
    }
}
