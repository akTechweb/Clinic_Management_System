using InfinityCoderzz_CMSV2026.Models;
using InfinityCoderzzz_CMSV2026.Repositories;

namespace InfinityCoderzzz_CMSV2026.Services
{
    public class PatientVisitService : IPatientVisitService
    {
        private readonly IPatientVisitRepository _patientVisitRepository;

        public PatientVisitService(IPatientVisitRepository patientVisitRepository)
        {
            _patientVisitRepository = patientVisitRepository;
        }

        #region Get All Patient Visits

        public List<PatientVisit> GetAllPatientVisits()
        {
            return _patientVisitRepository.GetAllPatientVisits();
        }

        #endregion

        #region Get Visit By ID

        public PatientVisit GetPatientVisitById(int visitId)
        {
            return _patientVisitRepository.GetPatientVisitById(visitId);
        }

        #endregion
    }
}