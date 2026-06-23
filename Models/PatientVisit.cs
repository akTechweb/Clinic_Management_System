namespace InfinityCoderzz_CMSV2026.Models
{
    public class PatientVisit
    {
        public int VisitId { get; set; }
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime? VisitDate { get; set; }
        //public string VisitStatus { get; set; }     // Waiting / In Progress / Completed

        // Display helpers
        //public string PatientName { get; set; }
        //public string DoctorName { get; set; }
        //public string DepartmentName { get; set; }
        //public string AppointmentNumber { get; set; }
        public int TokenNumber { get; set; }


        public string VisitStatus { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public string AppointmentNumber { get; set; } = string.Empty;
    }
}
