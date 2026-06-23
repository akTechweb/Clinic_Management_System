using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace InfinityCoderzz_CMSV2026.Controllers
{
    public class LoginController : Controller
    {
        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Please enter username and password.";
                return View();
            }

            string passwordHash = ComputeMd5Hash(password);
            string connStr = _configuration.GetConnectionString("DefaultConnection");

            await using SqlConnection con = new SqlConnection(connStr);
            await using SqlCommand cmd = new SqlCommand(@"
                SELECT 
                    u.UserId,
                    u.Username,
                    r.RoleId,
                    r.RoleName,
                    s.StaffId,
                    s.FullName,
                    d.DoctorId
                FROM dbo.Users u
                INNER JOIN dbo.Roles r ON u.RoleId = r.RoleId
                LEFT JOIN dbo.Staff s ON u.UserId = s.UserId
                LEFT JOIN dbo.Doctors d ON s.StaffId = d.StaffId
                WHERE u.Username = @Username
                  AND u.PasswordHash = @PasswordHash
                  AND u.IsActive = 1", con);

            cmd.Parameters.AddWithValue("@Username", username);
            cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);

            await con.OpenAsync();
            await using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                ViewBag.Error = "Invalid username or password.";
                return View();
            }

            int userId = Convert.ToInt32(reader["UserId"]);
            int roleId = Convert.ToInt32(reader["RoleId"]);
            string roleName = reader["RoleName"].ToString() ?? "";
            string fullName = reader["FullName"] == DBNull.Value ? username : reader["FullName"].ToString() ?? username;

            HttpContext.Session.SetInt32("UserId", userId);
            HttpContext.Session.SetInt32("RoleId", roleId);
            HttpContext.Session.SetString("UserName", username);
            HttpContext.Session.SetString("RoleName", roleName);
            HttpContext.Session.SetString("FullName", fullName);

            if (roleName == "Doctor" && reader["DoctorId"] != DBNull.Value)
            {
                HttpContext.Session.SetInt32("DoctorId", Convert.ToInt32(reader["DoctorId"]));
                return RedirectToAction("Dashboard", "Doctor");
            }

            if (roleName == "Receptionist")
            {
                return RedirectToAction("Dashboard", "Receptionists");
            }

            if (roleName == "Pharmacist")
            {
                int pharmacistStaffId = reader["StaffId"] == DBNull.Value ? userId : Convert.ToInt32(reader["StaffId"]);
                HttpContext.Session.SetInt32("PharmacistId", pharmacistStaffId);
                HttpContext.Session.SetString("PharmacistName", fullName);
                return RedirectToAction("Index", "PharmacyDashboard");
            }

            if (roleName == "Lab Technician")
            {
                return RedirectToAction("Dashboard", "LabTechnician");
            }

            if (roleName == "Admin")
            {
                return RedirectToAction("Dashboard", "Admin");
            }

            ViewBag.Error = "Role is not configured.";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }
        private static string ComputeMd5Hash(string input)
        {
            using var md5 = System.Security.Cryptography.MD5.Create();
            byte[] bytes = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));

            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }

            return builder.ToString();
        }

    }
}