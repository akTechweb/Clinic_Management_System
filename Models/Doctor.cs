namespace InfinityCoderzz_CMSV2026.Models
{
    public class Doctor
    {
        public int DoctorId { get; set; }
        public int StaffId { get; set; }
        public int DepartmentId { get; set; }
        public decimal ConsultationFee { get; set; }
        public int? ExperienceYears { get; set; }
        public bool IsActive { get; set; }

        // Display helpers (JOIN from Staff + Departments)
        public string FullName { get; set; }
        public string DepartmentName { get; set; }
        public string MobileNumber { get; set; }
    }
}
