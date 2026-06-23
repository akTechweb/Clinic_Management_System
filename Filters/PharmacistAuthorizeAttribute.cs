using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace InfinityCoderzz_CMSV2026.Filters
{
    /// <summary>
    /// Restricts an action/controller to a logged-in pharmacist
    /// (PharmacistId stored in session). Redirects to the pharmacy login otherwise.
    /// </summary>
    public class PharmacistAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            int pharmacistId = context.HttpContext.Session.GetInt32("PharmacistId") ?? 0;
            if (pharmacistId <= 0)
            {
                context.Result = new RedirectToActionResult("Index", "Login", null);
            }

            base.OnActionExecuting(context);
        }
    }
}
