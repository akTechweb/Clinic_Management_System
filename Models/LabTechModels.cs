using System;
using System.ComponentModel.DataAnnotations;

namespace InfinityCoderzz_CMSV2026.Models
{
    public class LabTechSessionModel
    {
        public int TechnicianId { get; set; }
        public string TechnicianName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
    }

    public class PendingLabRequestViewModel
    {
        public int RequestItemId { get; set; }
        public int RequestId { get; set; }
        public string MMRCode { get; set; }
        public string PatientName { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string MobileNumber { get; set; }
        public int TestId { get; set; }
        public string TestName { get; set; }
        public string NormalRange { get; set; }
        public decimal TestCharge { get; set; }
        public string DoctorName { get; set; }
        public DateTime RequestDate { get; set; }
        public string Status { get; set; }
    }

    public class LabPatientSearchViewModel
    {
        public string MMRCode { get; set; }
        public string FullName { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string MobileNumber { get; set; }
    }

    public class EnterLabResultViewModel
    {
        public int RequestItemId { get; set; }
        public int RequestId { get; set; }
        public int ConsultationId { get; set; }
        public int DoctorId { get; set; }
        public string MMRCode { get; set; }
        public string PatientName { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string MobileNumber { get; set; }
        public int TestId { get; set; }
        public string TestName { get; set; }
        
        public string NormalRange { get; set; }
        public decimal TestCharge { get; set; }
        public string DoctorName { get; set; }
       
        public string Status { get; set; }

        [Required(ErrorMessage = "Result value is required")]
        public string ResultValue { get; set; }
        public string Observation { get; set; }
        public string Remarks { get; set; }
        public bool IsAbnormal { get; set; }
       
        public string? DoctorEmail { get; set; }
        public string? ReportFilePath { get; set; }
        public string? TestDescription { get; set; } 
    }

    public class CompletedLabReportViewModel
    {
        public int ResultId { get; set; }
        public int RequestItemId { get; set; }
        public string MMRCode { get; set; }
        public string PatientName { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public int TestId { get; set; }
        public string TestName { get; set; }
        public string NormalRange { get; set; }
        public string ResultValue { get; set; }
        public string Observation { get; set; }
        public string Remarks { get; set; }
        public bool IsAbnormal { get; set; }
        public string ReportFilePath { get; set; }
        public DateTime ResultDate { get; set; }
        public string DoctorName { get; set; }
        public string DoctorEmail { get; set; }
        public string TechnicianName { get; set; }
    }

    public class LabResultPdfViewModel
    {
        public int ResultId { get; set; }
        public int RequestItemId { get; set; }
        public string MMRCode { get; set; }
        public string PatientName { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string MobileNumber { get; set; }
        public string PatientEmail { get; set; }
        public int TestId { get; set; }
        public string TestName { get; set; }
        //public string TestDescription { get; set; }
        public string NormalRange { get; set; }
        public string ResultValue { get; set; }
        public string Observation { get; set; }
        public string Remarks { get; set; }
        public bool IsAbnormal { get; set; }
        public DateTime ResultDate { get; set; }
        public string DoctorName { get; set; }
        //public string DoctorEmail { get; set; }
        public string TechnicianName { get; set; }
        
        public string? ReportFilePath { get; set; }
        public string? TestDescription { get; set; }
        public string? DoctorEmail { get; set; }
    }
    public class LabDashboardStatsViewModel
    {
        public int PendingTests { get; set; }
        public int CompletedTests { get; set; }
        public int TotalTestsToday { get; set; }
        public int TotalPatientsToday { get; set; }
        public int TotalRequests { get; set; }
        public int CompletedReports { get; set; }
        public int UnpaidBills { get; set; }
    }
}
