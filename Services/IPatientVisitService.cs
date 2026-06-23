using InfinityCoderzz_CMSV2026.Models;

namespace InfinityCoderzzz_CMSV2026.Services
{
    public interface IPatientVisitService
    {
        List<PatientVisit> GetAllPatientVisits();

        PatientVisit GetPatientVisitById(int visitId);
    }
}