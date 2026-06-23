namespace InfinityCoderzz_CMSV2026.Services.PharmacyServices
{
    public interface IPharmacyLoginService
    {
        (int StaffId, string FullName) Login(string username, string passwordHash);
    }
}
