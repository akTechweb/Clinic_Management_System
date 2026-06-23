namespace InfinityCoderzz_CMSV2026.Repositories.PharmacistRepositories
{
    public interface IPharmacyLoginRepository
    {
        (int StaffId, string FullName) Login(string username, string passwordHash);
    }
}
