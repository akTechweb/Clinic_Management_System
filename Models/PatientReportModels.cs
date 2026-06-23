using System;
using System.Collections.Generic;

namespace InfinityCoderzz_CMSV2026.Models
{
    public class PatientReportSearchViewModel
    {
        public string MMRCode { get; set; }
        public string FullName { get; set; }
        public string MobileNumber { get; set; }
    }

    public class LabReportItemViewModel
    {
        public string TestName { get; set; }
        public string ResultValue { get; set; }
        public string Observation { get; set; }
        public bool IsAbnormal { get; set; }
        public string NormalRange { get; set; }
        public DateTime ResultDate { get; set; }
        public string Status { get; set; }
        public string DoctorName { get; set; }
        public DateTime VisitDate { get; set; }
    }

    public class PatientFullReportViewModel
    {
        public string MMRCode { get; set; }
        public string PatientName { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public List<LabReportItemViewModel> LabResults { get; set; } = new();
    }
}
