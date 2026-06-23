using InfinityCoderzz_CMSV2026.Models;

namespace InfinityCoderzzz_CMSV2026.Repositories
{
    public interface IAppointmentRepository
    {
        List<Appointment> GetAllAppointments();

        Appointment GetAppointmentById(int appointmentId);

        List<Appointment> GetAppointmentsByFilter(
            int? departmentId,
            int? doctorId,
            string patientCode,
            DateTime? fromDate,
            DateTime? toDate);

        void BookAppointment(Appointment appointment, out string message);

        void CancelAppointment(int appointmentId, out string message);

        List<Doctor> GetAllActiveDoctors();
    }
}