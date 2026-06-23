namespace InfinityCoderzz_CMSV2026.Models
{
    public class DoctorSchedule
    {
        public int ScheduleId { get; set; }
        public int DoctorId { get; set; }
        public DateTime AvailableDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int MaxPatients { get; set; }

        // Display helpers
        public string DoctorName { get; set; }
        public int BookedCount { get; set; }
        public int AvailableSlots => MaxPatients - BookedCount;
    }
}
