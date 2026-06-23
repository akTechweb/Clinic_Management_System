using InfinityCoderzz_CMSV2026.Models;

namespace InfinityCoderzzz_CMSV2026.Repositories
{
    public interface IPatientRepository
    {
        List<Patient> GetAllPatients();
        Patient GetPatientById(int patientId);
        string GetNextPatientCode();
        void RegisterPatient(Patient patient);
        void UpdatePatient(Patient patient);
        List<Patient> SearchPatients(string keyword);
    }
}