using System.ComponentModel.DataAnnotations;

namespace InfinityCoderzz_CMSV2026.Models
{
    public class UserSessionModel
    {
        public int UserId { get; set; }
        public int DoctorId { get; set; }
        public string FullName { get; set; }
        public string RoleName { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string PasswordHash { get; set; }
    }
}
