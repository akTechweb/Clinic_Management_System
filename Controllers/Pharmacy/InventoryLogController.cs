using InfinityCoderzz_CMSV2026.Services.PharmacyServices;
using Microsoft.AspNetCore.Mvc;

namespace InfinityCoderzz_CMSV2026.Controllers.Pharmacy
{
    public class InventoryLogController : Controller
    {
        private readonly IInventoryLogService _service;

        public InventoryLogController(IInventoryLogService service)
        {
            _service = service;
        }

        public IActionResult List()
        {
            int pharmacistId = HttpContext.Session.GetInt32("PharmacistId") ?? 0;
            if (pharmacistId == 0)
                return RedirectToAction("Index", "Login");

            var logs = _service.GetInventoryLogs();
            return View(logs);
        }
    }
}
