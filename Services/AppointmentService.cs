using InfinityCoderzz_CMSV2026.Models;
using InfinityCoderzzz_CMSV2026.Repositories;

namespace InfinityCoderzzz_CMSV2026.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;

        public AppointmentService(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }

        public List<Appointment> GetAllAppointments()
        {
            return _appointmentRepository.GetAllAppointments();
        }

        public Appointment GetAppointmentById(int appointmentId)
        {
            return _appointmentRepository.GetAppointmentById(appointmentId);
        }

        public List<Appointment> GetAppointmentsByFilter(
            int? departmentId,
            int? doctorId,
            string patientCode,
            DateTime? fromDate,
            DateTime? toDate)
        {
            return _appointmentRepository.GetAppointmentsByFilter(
                departmentId,
                doctorId,
                patientCode,
                fromDate,
                toDate);
        }

        public void BookAppointment(Appointment appointment, out string message)
        {
            DateTime today = DateTime.Today;
            DateTime tomorrow = DateTime.Today.AddDays(1);

            if (appointment.PatientId <= 0)
            {
                message = "ERROR: Please select a valid patient.";
                return;
            }

            if (appointment.DoctorId <= 0)
            {
                message = "ERROR: Please select a doctor.";
                return;
            }

            if (appointment.AppointmentDate.Date != today &&
                appointment.AppointmentDate.Date != tomorrow)
            {
                message = "ERROR: Appointments can only be booked for today or tomorrow.";
                return;
            }

            if (appointment.AppointmentTime == TimeSpan.Zero)
            {
                message = "ERROR: Please select an appointment time.";
                return;
            }

            _appointmentRepository.BookAppointment(appointment, out message);
        }

        public void CancelAppointment(int appointmentId, out string message)
        {
            _appointmentRepository.CancelAppointment(appointmentId, out message);
        }

        public List<Doctor> GetAllActiveDoctors()
        {
            return _appointmentRepository.GetAllActiveDoctors();
        }
    }
}