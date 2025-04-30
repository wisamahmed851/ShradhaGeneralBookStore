using Microsoft.AspNetCore.Mvc;

namespace ShradhaGeneralBookStore.Controllers
{
    public class AuthApiController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
