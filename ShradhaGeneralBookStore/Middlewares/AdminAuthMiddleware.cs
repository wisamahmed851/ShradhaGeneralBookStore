using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ShradhaGeneralBookStore.Middlewares
{
    public class AdminAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public AdminAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Check if the request is for the Admin area
            var path = context.Request.Path;

            if (path.HasValue && path.Value.ToLower().StartsWith("/admin"))
            {
                var userRole = context.Session.GetString("UserRole");

                if (string.IsNullOrEmpty(userRole) || userRole != "Admin")
                {
                    context.Response.Redirect("/Home/Index?showLogin=true");
                    return;
                }
            }

            await _next(context);
        }
    }
}
