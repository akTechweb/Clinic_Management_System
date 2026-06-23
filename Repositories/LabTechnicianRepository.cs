using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using InfinityCoderzz_CMSV2026.Models;

namespace InfinityCoderzz_CMSV2026.Repositories
{
    public class LabTechnicianRepository : ILabTechnicianRepository
    {
        private readonly string _conn;
        public LabTechnicianRepository(IConfiguration cfg) =>
            _conn = cfg.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("DefaultConnection is missing in appsettings.json");

        private SqlConnection Open() => new SqlConnection(_conn);

        private SqlCommand SP(SqlConnection c, string name) =>
            new SqlCommand(name, c) { CommandType = CommandType.StoredProcedure };

        private static string Str(SqlDataReader r, string col) =>
            r[col] == DBNull.Value ? "" : r[col].ToString();


        // 2. Dashboard stats
        public async Task<LabDashboardStatsViewModel> GetDashboardStatsAsync()
        {
            await using var c = Open();
            await using var cmd = SP(c, "usp_GetLabDashboardStats");
            await c.OpenAsync();
            await using var r = await cmd.ExecuteReaderAsync();
            if (await r.ReadAsync())
                return new LabDashboardStatsViewModel
                {
                    TotalRequests = Convert.ToInt32(r["TotalRequests"]),
                    PendingTests = Convert.ToInt32(r["PendingTests"]),
                    CompletedReports = Convert.ToInt32(r["CompletedReports"]),
                    UnpaidBills = Convert.ToInt32(r["UnpaidBills"])
                };
            return new LabDashboardStatsViewModel();
        }

        // 3. Pending lab requests
        public async Task<List<PendingLabRequestViewModel>> GetPendingLabRequestsAsync(string searchMMR)
        {
            var list = new List<PendingLabRequestViewModel>();
            await using var c = Open();
            await using var cmd = SP(c, "usp_GetPendingLabRequests");
            cmd.Parameters.AddWithValue("@SearchMMR", (object?)searchMMR ?? DBNull.Value);
            await c.OpenAsync();
            await using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync())
                list.Add(new PendingLabRequestViewModel
                {
                    RequestItemId = Convert.ToInt32(r["RequestItemId"]),
                    RequestId = Convert.ToInt32(r["RequestId"]),
                    MMRCode = Str(r, "MMRCode"),
                    PatientName = Str(r, "PatientName"),
                    Age = Convert.ToInt32(r["Age"]),
                    Gender = Str(r, "Gender"),
                    MobileNumber = Str(r, "MobileNumber"),
                    TestId = Convert.ToInt32(r["TestId"]),
                    TestName = Str(r, "TestName"),
                    NormalRange = Str(r, "NormalRange"),
                    TestCharge = Convert.ToDecimal(r["TestCharge"]),
                    DoctorName = Str(r, "DoctorName"),
                    RequestDate = Convert.ToDateTime(r["RequestDate"]),
                    Status = Str(r, "Status")
                });
            return list;
        }

        // 4. Search patient by MMR
        public async Task<List<LabPatientSearchViewModel>> SearchPatientByMMRAsync(string searchMMR)
        {
            var list = new List<LabPatientSearchViewModel>();
            await using var c = Open();
            await using var cmd = SP(c, "usp_LabSearchPatientByMMR");
            cmd.Parameters.AddWithValue("@SearchMMR", searchMMR ?? "");
            await c.OpenAsync();
            await using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync())
                list.Add(new LabPatientSearchViewModel
                {
                    MMRCode = Str(r, "MMRCode"),
                    FullName = Str(r, "FullName"),
                    Age = Convert.ToInt32(r["Age"]),
                    Gender = Str(r, "Gender"),
                    MobileNumber = Str(r, "MobileNumber")
                });
            return list;
        }

        // 5. Get request item details
        public async Task<EnterLabResultViewModel> GetLabRequestItemDetailsAsync(int requestItemId)
        {
            await using var c = Open();
            await using var cmd = SP(c, "usp_GetLabRequestItemDetails");
            cmd.Parameters.AddWithValue("@RequestItemId", requestItemId);
            await c.OpenAsync();
            await using var r = await cmd.ExecuteReaderAsync();
            if (await r.ReadAsync())
                return new EnterLabResultViewModel
                {
                    RequestItemId = Convert.ToInt32(r["RequestItemId"]),
                    RequestId = Convert.ToInt32(r["RequestId"]),
                    ConsultationId = r["ConsultationId"] == DBNull.Value ? 0 : Convert.ToInt32(r["ConsultationId"]),
                    DoctorId = Convert.ToInt32(r["DoctorId"]),
                    MMRCode = Str(r, "MMRCode"),
                    PatientName = Str(r, "PatientName"),
                    Age = Convert.ToInt32(r["Age"]),
                    Gender = Str(r, "Gender"),
                    MobileNumber = Str(r, "MobileNumber"),
                    TestId = Convert.ToInt32(r["TestId"]),
                    TestName = Str(r, "TestName"),
                    TestDescription = Str(r, "TestDescription"),
                    NormalRange = Str(r, "NormalRange"),
                    TestCharge = Convert.ToDecimal(r["TestCharge"]),
                    DoctorName = Str(r, "DoctorName"),
                    DoctorEmail = Str(r, "DoctorEmail"),
                    Status = Str(r, "Status")
                };
            return null;
        }

        // 6. Add lab result
        public async Task<int> AddLabResultAsync(EnterLabResultViewModel m, int technicianId)
        {
            await using var c = Open();
            await using var cmd = SP(c, "usp_AddLabResult");
            cmd.Parameters.AddWithValue("@RequestItemId", m.RequestItemId);
            cmd.Parameters.AddWithValue("@ResultValue", m.ResultValue ?? "");
            cmd.Parameters.AddWithValue("@Observation", (object?)m.Observation ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Remarks", (object?)m.Remarks ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsAbnormal", m.IsAbnormal);
            cmd.Parameters.AddWithValue("@ReportFilePath", DBNull.Value);
            cmd.Parameters.AddWithValue("@EnteredBy", technicianId);
            await c.OpenAsync();
            await using var r = await cmd.ExecuteReaderAsync();
            await r.ReadAsync();
            return Convert.ToInt32(r["ResultId"]);
        }

        // 7. Completed reports
        public async Task<List<CompletedLabReportViewModel>> GetCompletedLabReportsAsync(string searchMMR)
        {
            var list = new List<CompletedLabReportViewModel>();
            await using var c = Open();
            await using var cmd = SP(c, "usp_GetCompletedLabReports");
            cmd.Parameters.AddWithValue("@SearchMMR", (object?)searchMMR ?? DBNull.Value);
            await c.OpenAsync();
            await using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync())
                list.Add(new CompletedLabReportViewModel
                {
                    ResultId = Convert.ToInt32(r["ResultId"]),
                    RequestItemId = Convert.ToInt32(r["RequestItemId"]),
                    MMRCode = Str(r, "MMRCode"),
                    PatientName = Str(r, "PatientName"),
                    Age = Convert.ToInt32(r["Age"]),
                    Gender = Str(r, "Gender"),
                    TestId = Convert.ToInt32(r["TestId"]),
                    TestName = Str(r, "TestName"),
                    NormalRange = Str(r, "NormalRange"),
                    ResultValue = Str(r, "ResultValue"),
                    Observation = Str(r, "Observation"),
                    Remarks = Str(r, "Remarks"),
                    IsAbnormal = Convert.ToBoolean(r["IsAbnormal"]),
                    ReportFilePath = Str(r, "ReportFilePath"),
                    ResultDate = Convert.ToDateTime(r["ResultDate"]),
                    DoctorName = Str(r, "DoctorName"),
                    DoctorEmail = Str(r, "DoctorEmail"),
                    TechnicianName = Str(r, "TechnicianName")
                });
            return list;
        }

        // 8. Single result details
        public async Task<LabResultPdfViewModel> GetLabResultDetailsAsync(int resultId)
        {
            await using var c = Open();
            await using var cmd = SP(c, "usp_GetLabResultDetails");
            cmd.Parameters.AddWithValue("@ResultId", resultId);
            await c.OpenAsync();
            await using var r = await cmd.ExecuteReaderAsync();
            if (await r.ReadAsync())
                return new LabResultPdfViewModel
                {
                    ResultId = Convert.ToInt32(r["ResultId"]),
                    MMRCode = Str(r, "MMRCode"),
                    PatientName = Str(r, "PatientName"),
                    Age = Convert.ToInt32(r["Age"]),
                    Gender = Str(r, "Gender"),
                    MobileNumber = Str(r, "MobileNumber"),
                    PatientEmail = Str(r, "PatientEmail"),
                    TestName = Str(r, "TestName"),
                    TestDescription = Str(r, "TestDescription"),
                    NormalRange = Str(r, "NormalRange"),
                    ResultValue = Str(r, "ResultValue"),
                    Observation = Str(r, "Observation"),
                    Remarks = Str(r, "Remarks"),
                    IsAbnormal = Convert.ToBoolean(r["IsAbnormal"]),
                    ResultDate = Convert.ToDateTime(r["ResultDate"]),
                    DoctorName = Str(r, "DoctorName"),
                    DoctorEmail = Str(r, "DoctorEmail"),
                    TechnicianName = Str(r, "TechnicianName")
                };
            return null;
        }

        // 9. Add report notification
        public async Task AddReportNotificationAsync(int resultId, string sentTo, string recipientType, string emailStatus)
        {
            await using var c = Open();
            await using var cmd = SP(c, "usp_AddReportNotification");
            cmd.Parameters.AddWithValue("@ResultId", resultId);
            cmd.Parameters.AddWithValue("@SentTo", sentTo ?? "");
            cmd.Parameters.AddWithValue("@RecipientType", recipientType ?? "Doctor");
            cmd.Parameters.AddWithValue("@EmailStatus", emailStatus ?? "Sent");
            await c.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        // 10. Unbilled requests
        public async Task<List<UnbilledLabRequestViewModel>> GetUnbilledLabRequestsAsync(string searchMMR)
        {
            var list = new List<UnbilledLabRequestViewModel>();
            await using var c = Open();
            await using var cmd = SP(c, "usp_GetUnbilledLabRequests");
            cmd.Parameters.AddWithValue("@SearchMMR", (object?)searchMMR ?? DBNull.Value);
            await c.OpenAsync();
            await using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync())
                list.Add(new UnbilledLabRequestViewModel
                {
                    RequestId = Convert.ToInt32(r["RequestId"]),
                    MMRCode = Str(r, "MMRCode"),
                    PatientName = Str(r, "PatientName"),
                    MobileNumber = Str(r, "MobileNumber"),
                    RequestDate = Convert.ToDateTime(r["RequestDate"]),
                    TotalTests = Convert.ToInt32(r["TotalTests"]),
                    TotalAmount = Convert.ToDecimal(r["TotalAmount"])
                });
            return list;
        }

        // 11. Generate bill
        public async Task<int> GenerateLabBillAsync(int requestId, int technicianId)
        {
            await using var c = Open();
            await using var cmd = SP(c, "usp_GenerateLabBill");
            cmd.Parameters.AddWithValue("@RequestId", requestId);
            cmd.Parameters.AddWithValue("@GeneratedBy", technicianId);
            await c.OpenAsync();
            await using var r = await cmd.ExecuteReaderAsync();
            await r.ReadAsync();
            return Convert.ToInt32(r["BillId"]);
        }

        // 12. Get all bills
        public async Task<List<LabBillViewModel>> GetLabBillsAsync(string searchMMR)
        {
            var list = new List<LabBillViewModel>();
            await using var c = Open();
            await using var cmd = SP(c, "usp_GetLabBills");
            cmd.Parameters.AddWithValue("@SearchMMR", (object?)searchMMR ?? DBNull.Value);
            await c.OpenAsync();
            await using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync())
                list.Add(new LabBillViewModel
                {
                    BillId = Convert.ToInt32(r["BillId"]),
                    RequestId = Convert.ToInt32(r["RequestId"]),
                    MMRCode = Str(r, "MMRCode"),
                    PatientName = Str(r, "PatientName"),
                    MobileNumber = Str(r, "MobileNumber"),
                    BillDate = Convert.ToDateTime(r["BillDate"]),
                    TotalAmount = Convert.ToDecimal(r["TotalAmount"]),
                    PaymentStatus = Str(r, "PaymentStatus")
                });
            return list;
        }

        // 13. Bill details
        public async Task<LabBillDetailsViewModel> GetLabBillDetailsAsync(int billId)
        {
            var vm = new LabBillDetailsViewModel();
            await using var c = Open();
            await using var cmd = SP(c, "usp_GetLabBillDetails");
            cmd.Parameters.AddWithValue("@BillId", billId);
            await c.OpenAsync();
            await using var r = await cmd.ExecuteReaderAsync();
            if (await r.ReadAsync())
            {
                vm.Bill = new LabBillViewModel
                {
                    BillId = Convert.ToInt32(r["BillId"]),
                    RequestId = Convert.ToInt32(r["RequestId"]),
                    MMRCode = Str(r, "MMRCode"),
                    PatientName = Str(r, "PatientName"),
                    MobileNumber = Str(r, "MobileNumber"),
                    BillDate = Convert.ToDateTime(r["BillDate"]),
                    TotalAmount = Convert.ToDecimal(r["TotalAmount"]),
                    PaymentStatus = Str(r, "PaymentStatus")
                };
                vm.Age = Convert.ToInt32(r["Age"]);
                vm.Gender = Str(r, "Gender");
            }
            if (await r.NextResultAsync())
                while (await r.ReadAsync())
                    vm.Items.Add(new LabBillLineItemViewModel
                    {
                        TestName = Str(r, "TestName"),
                        Amount = Convert.ToDecimal(r["Amount"])
                    });
            return vm;
        }

        // 14. Update payment status
        public async Task UpdateLabBillPaymentStatusAsync(int billId, string paymentStatus)
        {
            await using var c = Open();
            await using var cmd = SP(c, "usp_UpdateLabBillPaymentStatus");
            cmd.Parameters.AddWithValue("@BillId", billId);
            cmd.Parameters.AddWithValue("@PaymentStatus", paymentStatus ?? "Unpaid");
            await c.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }
    }
}