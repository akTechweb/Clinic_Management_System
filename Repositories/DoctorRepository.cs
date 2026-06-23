using System;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using InfinityCoderzz_CMSV2026.Models;

namespace InfinityCoderzz_CMSV2026.Repositories
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly string _conn;
        public DoctorRepository(IConfiguration cfg) =>
            _conn = cfg.GetConnectionString("DefaultConnection");

        // ── helpers ─────────────────────────────────────────────────────────
        private SqlConnection Open() => new SqlConnection(_conn);

        private SqlCommand SP(SqlConnection c, string name)
        {
            var cmd = new SqlCommand(name, c) { CommandType = CommandType.StoredProcedure };
            return cmd;
        }

        // ── 1. Login ─────────────────────────────────────────────────────────
        public async Task<UserSessionModel> AuthenticateDoctorAsync(LoginViewModel m)
        {
            try
            {
                await using var c = Open();
                await using var cmd = SP(c, "usp_DoctorLogin");
                cmd.Parameters.AddWithValue("@Username", m.Username ?? "");
                cmd.Parameters.AddWithValue("@PasswordHash", m.PasswordHash ?? "");
                await c.OpenAsync();
                await using var r = await cmd.ExecuteReaderAsync();
                if (await r.ReadAsync())
                    return new UserSessionModel
                    {
                        UserId = Convert.ToInt32(r["UserId"]),
                        DoctorId = Convert.ToInt32(r["DoctorId"]),
                        FullName = r["FullName"].ToString(),
                        RoleName = r["RoleName"].ToString()
                    };
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"=== DB ERROR: {ex.Message} ===");
                throw;
            }
        }

        // ── 2. Dashboard stats ───────────────────────────────────────────────
        public async Task<DashboardStatsViewModel> GetDashboardStatsAsync(int doctorId)
        {
            await using var c = Open();
            await using var cmd = SP(c, "usp_GetDoctorDashboardStats");
            cmd.Parameters.AddWithValue("@DoctorId", doctorId);
            await c.OpenAsync();
            await using var r = await cmd.ExecuteReaderAsync();
            if (await r.ReadAsync())
                return new DashboardStatsViewModel
                {
                    TodayTotal = Convert.ToInt32(r["TodayTotal"]),
                    TodayPending = Convert.ToInt32(r["TodayPending"]),
                    ConsultationsDone = Convert.ToInt32(r["ConsultationsDone"])
                };
            return new DashboardStatsViewModel();
        }

        // ── 3. Today appointments ────────────────────────────────────────────
        public async Task<List<AppointmentViewModel>> GetTodaysAppointmentsAsync(int doctorId)
        {
            var list = new List<AppointmentViewModel>();
            await using var c = Open();
            await using var cmd = SP(c, "usp_GetTodayAppointments");
            cmd.Parameters.AddWithValue("@DoctorId", doctorId);
            await c.OpenAsync();
            await using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync())
                list.Add(MapAppointment(r));
            return list;
        }

        // ── 4. Tomorrow appointments ─────────────────────────────────────────
        public async Task<List<AppointmentViewModel>> GetTomorrowAppointmentsAsync(int doctorId)
        {
            var list = new List<AppointmentViewModel>();
            await using var c = Open();
            await using var cmd = SP(c, "usp_GetTomorrowAppointments");
            cmd.Parameters.AddWithValue("@DoctorId", doctorId);
            await c.OpenAsync();
            await using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync())
                list.Add(MapAppointment(r));
            return list;
        }

        private static AppointmentViewModel MapAppointment(SqlDataReader r) => new()
        {
            AppointmentId = Convert.ToInt32(r["AppointmentId"]),
            TokenNumber = Convert.ToInt32(r["TokenNumber"]),
            MMRCode = r["MMRCode"].ToString(),
            FullName = r["FullName"].ToString(),
            Age = Convert.ToInt32(r["Age"]),
            Gender = r["Gender"].ToString(),
            MobileNumber = r["MobileNumber"].ToString(),
            AppointmentTime = r["AppointmentTime"].ToString(),
            Status = r["Status"].ToString()
        };

        // ── 5. Consultation setup data ───────────────────────────────────────
        public async Task<ConsultationSetupViewModel> GetConsultationSetupDataAsync(int appointmentId)
        {
            var vm = new ConsultationSetupViewModel { AppointmentId = appointmentId };

            await using var c = Open();
            await c.OpenAsync();

            // Patient details from appointment
            await using (var cmd = SP(c, "usp_GetConsultationSetupData"))
            {
                cmd.Parameters.AddWithValue("@AppointmentId", appointmentId);
                await using var r = await cmd.ExecuteReaderAsync();
                if (await r.ReadAsync())
                {
                    vm.MMRCode = r["MMRCode"].ToString();
                    vm.FullName = r["FullName"].ToString();
                    vm.Age = Convert.ToInt32(r["Age"]);
                    vm.Gender = r["Gender"].ToString();
                    vm.MobileNumber = r["MobileNumber"].ToString();
                }
            }

            // Patient history
            await using (var cmd = SP(c, "usp_GetPatientHistory"))
            {
                cmd.Parameters.AddWithValue("@MMRCode", vm.MMRCode ?? "");
                await using var r = await cmd.ExecuteReaderAsync();
                while (await r.ReadAsync())
                {
                    vm.HistoricalData.Consultations.Add(new HistoricalConsultationItemViewModel
                    {
                        VisitDate = Convert.ToDateTime(r["VisitDate"]),
                        Diagnosis = r["Diagnosis"] == DBNull.Value ? "" : r["Diagnosis"].ToString(),
                        Symptoms = r["Symptoms"] == DBNull.Value ? "" : r["Symptoms"].ToString()
                    });
                }
            }

            // Medicines dropdown
            await using (var cmd = SP(c, "usp_GetMedicines"))
            {
                await using var r = await cmd.ExecuteReaderAsync();
                while (await r.ReadAsync())
                    vm.AvailableMedicines.Add(new MedicineLookupModel
                    {
                        MedicineId = Convert.ToInt32(r["MedicineId"]),
                        MedicineName = r["MedicineName"].ToString()
                    });
            }

            // Lab tests dropdown
            await using (var cmd = SP(c, "usp_GetLabTests"))
            {
                await using var r = await cmd.ExecuteReaderAsync();
                while (await r.ReadAsync())
                    vm.AvailableLabTests.Add(new LabTestLookupModel
                    {
                        TestId = Convert.ToInt32(r["TestId"]),
                        TestName = r["TestName"].ToString()
                    });
            }

            return vm;
        }

        // ── 6. Save full consultation ────────────────────────────────────────
        public async Task<FinalSummaryDocumentViewModel> SaveFullConsultationAsync(
            ConsultationSetupViewModel model, int doctorId)
        {
            await using var c = Open();
            await c.OpenAsync();

            // a) Create Visit
            int visitId;
            await using (var cmd = SP(c, "usp_CreateVisit"))
            {
                cmd.Parameters.AddWithValue("@AppointmentId", model.AppointmentId);
                cmd.Parameters.AddWithValue("@MMRCode", model.MMRCode);
                cmd.Parameters.AddWithValue("@DoctorId", doctorId);
                await using var r = await cmd.ExecuteReaderAsync();
                await r.ReadAsync();
                visitId = Convert.ToInt32(r["VisitId"]);
            }

            // b) Save Consultation
            int consultationId;
            await using (var cmd = SP(c, "usp_SaveConsultation"))
            {
                cmd.Parameters.AddWithValue("@VisitId", visitId);
                cmd.Parameters.AddWithValue("@Symptoms", model.Symptoms ?? "");
                cmd.Parameters.AddWithValue("@Diagnosis", model.Diagnosis ?? "");
                cmd.Parameters.AddWithValue("@Notes", (object?)model.Notes ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FollowUpDate", model.FollowUpDate.HasValue
                    ? (object)model.FollowUpDate.Value : DBNull.Value);
                await using var r = await cmd.ExecuteReaderAsync();
                await r.ReadAsync();
                consultationId = Convert.ToInt32(r["ConsultationId"]);
            }

            // c) Prescription header
            var prescSummary = new List<PrescriptionSummaryLine>();
            if (model.PrescriptionItems != null && model.PrescriptionItems.Count > 0)
            {
                int prescriptionId;
                await using (var cmd = SP(c, "usp_CreatePrescription"))
                {
                    cmd.Parameters.AddWithValue("@ConsultationId", consultationId);
                    cmd.Parameters.AddWithValue("@DoctorId", doctorId);
                    cmd.Parameters.AddWithValue("@MMRCode", model.MMRCode);
                    await using var r = await cmd.ExecuteReaderAsync();
                    await r.ReadAsync();
                    prescriptionId = Convert.ToInt32(r["PrescriptionId"]);
                }

                // d) Prescription items
                foreach (var item in model.PrescriptionItems)
                {
                    string medName = item.MedicineId.ToString(); // will be replaced by lookup if needed
                    // lookup name from available list if passed via hidden
                    await using var cmd = SP(c, "usp_AddPrescriptionItem");
                    cmd.Parameters.AddWithValue("@PrescriptionId", prescriptionId);
                    cmd.Parameters.AddWithValue("@MedicineId", item.MedicineId);
                    cmd.Parameters.AddWithValue("@Dosage", item.Dosage ?? "");
                    cmd.Parameters.AddWithValue("@Frequency", item.Frequency ?? "");
                    cmd.Parameters.AddWithValue("@Duration", item.Duration ?? "");
                    cmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                    cmd.Parameters.AddWithValue("@Instructions", (object?)(item.Instructions) ?? DBNull.Value);
                    await cmd.ExecuteNonQueryAsync();

                    prescSummary.Add(new PrescriptionSummaryLine
                    {
                        MedicineName = "Medicine #" + item.MedicineId,
                        Dosage = item.Dosage,
                        Frequency = item.Frequency + "x/day",
                        Duration = item.Duration + " days",
                        Quantity = item.Quantity
                    });
                }
            }

            // e) Lab request
            var labTestNames = new List<string>();
            if (model.SelectedLabTests != null && model.SelectedLabTests.Count > 0)
            {
                int requestId;
                await using (var cmd = SP(c, "usp_CreateLabRequest"))
                {
                    cmd.Parameters.AddWithValue("@ConsultationId", consultationId);
                    cmd.Parameters.AddWithValue("@DoctorId", doctorId);
                    cmd.Parameters.AddWithValue("@MMRCode", model.MMRCode);
                    await using var r = await cmd.ExecuteReaderAsync();
                    await r.ReadAsync();
                    requestId = Convert.ToInt32(r["RequestId"]);
                }

                foreach (var testId in model.SelectedLabTests)
                {
                    await using var cmd = SP(c, "usp_AddLabRequestItem");
                    cmd.Parameters.AddWithValue("@RequestId", requestId);
                    cmd.Parameters.AddWithValue("@TestId", testId);
                    await cmd.ExecuteNonQueryAsync();
                    labTestNames.Add("Test #" + testId);
                }
            }

            // f) Complete consultation status
            await using (var cmd = SP(c, "usp_CompleteConsultation"))
            {
                cmd.Parameters.AddWithValue("@VisitId", visitId);
                cmd.Parameters.AddWithValue("@AppointmentId", model.AppointmentId);
                await cmd.ExecuteNonQueryAsync();
            }

            return new FinalSummaryDocumentViewModel
            {
                MMRCode = model.MMRCode,
                PatientName = model.FullName,
                Symptoms = model.Symptoms,
                Diagnosis = model.Diagnosis,
                ClinicalNotes = model.Notes,
                FollowUpDate = model.FollowUpDate,
                PrescribedMedicines = prescSummary,
                OrderedLabTests = labTestNames
            };
        }

        // ── 7. Search patients ───────────────────────────────────────────────
        public async Task<List<PatientReportSearchViewModel>> SearchPatientsAsync(string searchTerm)
        {
            var list = new List<PatientReportSearchViewModel>();
            await using var c = Open();
            await using var cmd = SP(c, "usp_SearchPatient");
            cmd.Parameters.AddWithValue("@SearchText", searchTerm ?? "");
            await c.OpenAsync();
            await using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync())
                list.Add(new PatientReportSearchViewModel
                {
                    MMRCode = r["MMRCode"].ToString(),
                    FullName = r["FullName"].ToString(),
                    MobileNumber = r["MobileNumber"].ToString()
                });
            return list;
        }

        // ── 8. Full patient report ───────────────────────────────────────────
        public async Task<PatientFullReportViewModel> GetPatientFullReportAsync(string mmrCode)
        {
            var vm = new PatientFullReportViewModel { MMRCode = mmrCode };
            await using var c = Open();
            await c.OpenAsync();

            await using var cmd = SP(c, "usp_GetPatientReports");
            cmd.Parameters.AddWithValue("@MMRCode", mmrCode);
            await using var r = await cmd.ExecuteReaderAsync();
            bool first = true;
            while (await r.ReadAsync())
            {
                if (first)
                {
                    vm.PatientName = r["FullName"].ToString();
                    vm.Age = Convert.ToInt32(r["Age"]);
                    vm.Gender = r["Gender"].ToString();
                    first = false;
                }
                vm.LabResults.Add(new LabReportItemViewModel
                {
                    TestName = r["TestName"].ToString(),
                    NormalRange = r["NormalRange"].ToString(),
                    ResultValue = r["ResultValue"].ToString(),
                    Observation = r["Observation"].ToString(),
                    IsAbnormal = Convert.ToBoolean(r["IsAbnormal"]),
                    ResultDate = Convert.ToDateTime(r["ResultDate"]),
                    Status = r["LabStatus"].ToString(),
                    DoctorName = r["DoctorName"].ToString(),
                    VisitDate = Convert.ToDateTime(r["VisitDate"])
                });
            }
            return vm;
        }
    }
}