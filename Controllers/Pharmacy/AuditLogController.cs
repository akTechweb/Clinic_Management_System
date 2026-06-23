using InfinityCoderzz_CMSV2026.Filters;
using InfinityCoderzz_CMSV2026.Services.PharmacyServices;
using Microsoft.AspNetCore.Mvc;

namespace InfinityCoderzz_CMSV2026.Controllers.Pharmacy
{
    [PharmacistAuthorize]
    public class AuditLogController : Controller
    {
        private readonly IAuditLogService _service;

        public AuditLogController(IAuditLogService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult List(DateTime? fromDate, DateTime? toDate)
        {
            ViewData["ActiveMenu"] = "AuditLogs";
            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");

            var logs = _service.GetAuditLogs(fromDate, toDate);
            return View(logs);
        }
    }
}
