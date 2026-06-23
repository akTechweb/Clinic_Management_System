using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InfinityCoderzz_CMSV2026.Models
{
    // ─── Session / Auth ───────────────────────────────────────────────────────
    public class UserSessionModel
    {
        public int    UserId    { get; set; }
        public int    DoctorId  { get; set; }
        public string FullName  { get; set; }
        public string RoleName  { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        public string Username     { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string PasswordHash { get; set; }
    }

    // ─── Lookup lists ─────────────────────────────────────────────────────────
    public class MedicineLookupModel
    {
        public int    MedicineId   { get; set; }
        public string MedicineName { get; set; }
    }

    public class LabTestLookupModel
    {
        public int    TestId   { get; set; }
        public string TestName { get; set; }
    }

    // ─── Appointment ──────────────────────────────────────────────────────────
    public class AppointmentViewModel
    {
        public int    AppointmentId   { get; set; }
        public int    TokenNumber     { get; set; }
        public string MMRCode         { get; set; }
        public string FullName        { get; set; }
        public string MobileNumber    { get; set; }
        public string Gender          { get; set; }
        public int    Age             { get; set; }
        public string AppointmentTime { get; set; }
        public string Status          { get; set; }
    }

    // ─── Prescription line input ──────────────────────────────────────────────
    public class PrescriptionItemInput
    {
        [Required] public int    MedicineId   { get; set; }
        [Required] public string Dosage       { get; set; }   // e.g. "1-0-1"
        [Required] public string Frequency    { get; set; }   // "1","2","3"
        [Required] public string Duration     { get; set; }   // days as string
        [Required] public int    Quantity     { get; set; }
        public string Instructions { get; set; }
    }

    // ─── Patient history ──────────────────────────────────────────────────────
    public class HistoricalConsultationItemViewModel
    {
        public DateTime VisitDate  { get; set; }
        public string   Diagnosis  { get; set; }
        public string   Symptoms   { get; set; }
        public string   DoctorName { get; set; }
    }

    public class PatientHistoryContainerViewModel
    {
        public List<HistoricalConsultationItemViewModel> Consultations { get; set; } = new();
    }

    // ─── Start Consultation page ──────────────────────────────────────────────
    public class ConsultationSetupViewModel
    {
        public int    AppointmentId { get; set; }
        public string MMRCode       { get; set; }
        public string FullName      { get; set; }
        public string Gender        { get; set; }
        public int    Age           { get; set; }
        public string MobileNumber  { get; set; }

        [Required(ErrorMessage = "Symptoms required")]
        public string Symptoms    { get; set; }

        [Required(ErrorMessage = "Diagnosis required")]
        public string Diagnosis   { get; set; }

        public string   Notes       { get; set; }
        public DateTime? FollowUpDate { get; set; }

        public List<int>                     SelectedLabTests     { get; set; } = new();
        public List<PrescriptionItemInput>   PrescriptionItems    { get; set; } = new();

        // dropdown data
        public List<MedicineLookupModel> AvailableMedicines { get; set; } = new();
        public List<LabTestLookupModel>  AvailableLabTests  { get; set; } = new();
        public PatientHistoryContainerViewModel HistoricalData { get; set; } = new();
    }

    // ─── Consultation outcome summary (report page) ───────────────────────────
    public class FinalSummaryDocumentViewModel
    {
        public string    MMRCode      { get; set; }
        public string    PatientName  { get; set; }
        public string    Symptoms     { get; set; }
        public string    Diagnosis    { get; set; }
        public string    ClinicalNotes { get; set; }
        public DateTime? FollowUpDate  { get; set; }

        // Prescription lines for the summary
        public List<PrescriptionSummaryLine> PrescribedMedicines { get; set; } = new();
        // Lab tests ordered
        public List<string> OrderedLabTests { get; set; } = new();
    }

    public class PrescriptionSummaryLine
    {
        public string MedicineName { get; set; }
        public string Dosage       { get; set; }
        public string Frequency    { get; set; }
        public string Duration     { get; set; }
        public int    Quantity     { get; set; }
    }

    // ─── Patient report / history search ─────────────────────────────────────
    public class PatientReportSearchViewModel
    {
        public string MMRCode      { get; set; }
        public string FullName     { get; set; }
        public string MobileNumber { get; set; }
    }

    public class LabReportItemViewModel
    {
        public string   TestName    { get; set; }
        public string   ResultValue { get; set; }
        public string   Observation { get; set; }
        public bool     IsAbnormal  { get; set; }
        public string   NormalRange { get; set; }
        public DateTime ResultDate  { get; set; }
        public string   Status      { get; set; }
        public string   DoctorName  { get; set; }
        public DateTime VisitDate   { get; set; }
    }

    // Report PDF view model
    public class PatientFullReportViewModel
    {
        public string MMRCode      { get; set; }
        public string PatientName  { get; set; }
        public int    Age          { get; set; }
        public string Gender       { get; set; }
        public List<LabReportItemViewModel> LabResults { get; set; } = new();
    }

    // ─── Dashboard stats ──────────────────────────────────────────────────────
    public class DashboardStatsViewModel
    {
        public int TodayTotal        { get; set; }
        public int TodayPending      { get; set; }
        public int ConsultationsDone { get; set; }
    }
}
