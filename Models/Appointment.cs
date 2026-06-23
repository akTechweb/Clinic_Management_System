using System;

namespace InfinityCoderzz_CMSV2026.Models
{
    public class Appointment
    {
        public int AppointmentId { get; set; }
        public string AppointmentNumber { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public int TokenNumber { get; set; }
        public string Status { get; set; }          // Scheduled / Completed / Cancelled
        public DateTime? CreatedAt { get; set; }

        // Display helpers (filled by JOIN queries)
        public string PatientName { get; set; }
        public string PatientCode { get; set; }
        public string DoctorName { get; set; }
        public string DepartmentName { get; set; }
        public decimal ConsultationFee { get; set; }
    }
}