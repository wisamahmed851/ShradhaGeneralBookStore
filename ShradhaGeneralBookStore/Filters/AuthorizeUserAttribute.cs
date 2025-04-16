using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ShradhaGeneralBookStore.Filters
{
    public class AuthorizeUserAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var userId = session.GetInt32("UserId");

            if (!userId.HasValue)
            {
                context.Result = new RedirectToActionResult("Index", "Home", new { showLogin = true });
            }

            base.OnActionExecuting(context);
        }
    }
}
