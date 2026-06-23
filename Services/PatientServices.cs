using InfinityCoderzz_CMSV2026.Models;
using InfinityCoderzzz_CMSV2026.Repositories;

namespace InfinityCoderzzz_CMSV2026.Services
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;

        public PatientService(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }

        public List<Patient> GetAllPatients()
        {
            return _patientRepository.GetAllPatients();
        }

        public Patient GetPatientById(int patientId)
        {
            return _patientRepository.GetPatientById(patientId);
        }

        public string GetNextPatientCode()
        {
            return _patientRepository.GetNextPatientCode();
        }

        public void RegisterPatient(Patient patient)
        {
            _patientRepository.RegisterPatient(patient);
        }

        public void UpdatePatient(Patient patient)
        {
            _patientRepository.UpdatePatient(patient);
        }

        public List<Patient> SearchPatients(string keyword)
        {
            return _patientRepository.SearchPatients(keyword);
        }
    }
}