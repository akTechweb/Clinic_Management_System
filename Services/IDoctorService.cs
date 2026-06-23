using System.Collections.Generic;
using System.Threading.Tasks;
using InfinityCoderzz_CMSV2026.Models;

namespace InfinityCoderzz_CMSV2026.Services
{
    public interface IDoctorService
    {
        Task<UserSessionModel>              AuthenticateDoctorAsync(LoginViewModel model);
        Task<DashboardStatsViewModel>       GetDashboardStatsAsync(int doctorId);
        Task<List<AppointmentViewModel>>    GetTodaysAppointmentsAsync(int doctorId);
        Task<List<AppointmentViewModel>>    GetTomorrowAppointmentsAsync(int doctorId);
        Task<ConsultationSetupViewModel>    GetConsultationSetupDataAsync(int appointmentId);
        Task<FinalSummaryDocumentViewModel> SaveFullConsultationAsync(ConsultationSetupViewModel model, int doctorId);
        Task<List<PatientReportSearchViewModel>> SearchPatientsAsync(string searchTerm);
        Task<PatientFullReportViewModel>    GetPatientFullReportAsync(string mmrCode);
    }
}
