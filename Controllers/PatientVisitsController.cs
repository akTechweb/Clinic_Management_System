using InfinityCoderzz_CMSV2026.Models;
using InfinityCoderzzz_CMSV2026.Services;
using Microsoft.AspNetCore.Mvc;

namespace InfinityCoderzz_CMSV2026.Controllers
{
    public class PatientVisitsController : Controller
    {
        private readonly IPatientVisitService _patientVisitService;

        public PatientVisitsController(IPatientVisitService patientVisitService)
        {
            _patientVisitService = patientVisitService;
        }

        // GET: PatientVisits
        public IActionResult Index()
        {
            List<PatientVisit> visits = _patientVisitService.GetAllPatientVisits();
            return View(visits);
        }

        // GET: PatientVisits/Details/5
        public IActionResult Details(int id)
        {
            PatientVisit visit = _patientVisitService.GetPatientVisitById(id);

            if (visit == null)
            {
                return NotFound();
            }

            return View(visit);
        }
    }
}