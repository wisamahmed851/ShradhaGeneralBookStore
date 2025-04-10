using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ShradhaGeneralBookStore.Filters
{
    public class AuthorizeAdminAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContext = context.HttpContext;
            var session = httpContext.Session;

            var userRole = session.GetString("UserRole");

            if (string.IsNullOrEmpty(userRole) || userRole != "Admin")
            {
                context.Result = new RedirectToActionResult("Index", "Home", new { showLogin = true })
                {
                    RouteValues = { ["area"] = "" } // Important!
                };
            }

            base.OnActionExecuting(context);
        }
    }
}
