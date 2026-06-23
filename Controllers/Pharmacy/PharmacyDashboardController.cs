using InfinityCoderzz_CMSV2026.Services.PharmacyServices;
using Microsoft.AspNetCore.Mvc;

namespace InfinityCoderzz_CMSV2026.Controllers.Pharmacy
{
    public class PharmacyDashboardController : Controller
    {
        private readonly IPharmacyDashboardService _dashboardService;

        public PharmacyDashboardController(IPharmacyDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            int pharmacistId = HttpContext.Session.GetInt32("PharmacistId") ?? 0;
            if (pharmacistId == 0)
                return RedirectToAction("Index", "Login");

            var dashboard = _dashboardService.GetDashboardData();
            return View(dashboard);
        }
    }
}
