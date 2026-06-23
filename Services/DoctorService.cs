using System.Collections.Generic;
using System.Threading.Tasks;
using InfinityCoderzz_CMSV2026.Models;
using InfinityCoderzz_CMSV2026.Repositories;

namespace InfinityCoderzz_CMSV2026.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _repo;
        public DoctorService(IDoctorRepository repo) => _repo = repo;

        public Task<UserSessionModel>              AuthenticateDoctorAsync(LoginViewModel m)         => _repo.AuthenticateDoctorAsync(m);
        public Task<DashboardStatsViewModel>       GetDashboardStatsAsync(int d)                     => _repo.GetDashboardStatsAsync(d);
        public Task<List<AppointmentViewModel>>    GetTodaysAppointmentsAsync(int d)                 => _repo.GetTodaysAppointmentsAsync(d);
        public Task<List<AppointmentViewModel>>    GetTomorrowAppointmentsAsync(int d)               => _repo.GetTomorrowAppointmentsAsync(d);
        public Task<ConsultationSetupViewModel>    GetConsultationSetupDataAsync(int a)              => _repo.GetConsultationSetupDataAsync(a);
        public Task<FinalSummaryDocumentViewModel> SaveFullConsultationAsync(ConsultationSetupViewModel m, int d) => _repo.SaveFullConsultationAsync(m, d);
        public Task<List<PatientReportSearchViewModel>> SearchPatientsAsync(string s)                => _repo.SearchPatientsAsync(s);
        public Task<PatientFullReportViewModel>    GetPatientFullReportAsync(string mm)              => _repo.GetPatientFullReportAsync(mm);
    }
}
