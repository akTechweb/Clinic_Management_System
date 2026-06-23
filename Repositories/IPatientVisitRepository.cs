using InfinityCoderzz_CMSV2026.Models;

namespace InfinityCoderzzz_CMSV2026.Repositories
{
    public interface IPatientVisitRepository
    {
        // List all patient visits
        List<PatientVisit> GetAllPatientVisits();

        // Get visit by ID
        PatientVisit GetPatientVisitById(int visitId);
    }
}