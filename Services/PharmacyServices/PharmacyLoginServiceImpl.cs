using InfinityCoderzz_CMSV2026.Repositories.PharmacistRepositories;

namespace InfinityCoderzz_CMSV2026.Services.PharmacyServices
{
    public class PharmacyLoginServiceImpl : IPharmacyLoginService
    {
        private readonly IPharmacyLoginRepository _repository;

        public PharmacyLoginServiceImpl(IPharmacyLoginRepository repository)
        {
            _repository = repository;
        }

        public (int StaffId, string FullName) Login(string username, string passwordHash)
        {
            return _repository.Login(username, passwordHash);
        }
    }
}
