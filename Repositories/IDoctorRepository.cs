using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InfinityCoderzz_CMSV2026.Models;

namespace InfinityCoderzz_CMSV2026.Repositories
{
    public interface IDoctorRepository
    {
        // Auth
        Task<UserSessionModel> AuthenticateDoctorAsync(LoginViewModel model);

        // Dashboard stats
        Task<DashboardStatsViewModel> GetDashboardStatsAsync(int doctorId);

        // Appointments
        Task<List<AppointmentViewModel>> GetTodaysAppointmentsAsync(int doctorId);
        Task<List<AppointmentViewModel>> GetTomorrowAppointmentsAsync(int doctorId);

        // Consultation setup
        Task<ConsultationSetupViewModel> GetConsultationSetupDataAsync(int appointmentId);

        // Save full consultation (visit + consultation + prescription + lab)
        Task<FinalSummaryDocumentViewModel> SaveFullConsultationAsync(ConsultationSetupViewModel model, int doctorId);

        // Patient History & Reports
        Task<List<PatientReportSearchViewModel>> SearchPatientsAsync(string searchTerm);
        Task<PatientFullReportViewModel> GetPatientFullReportAsync(string mmrCode);
    }
}
