using Microsoft.AspNetCore.Mvc;

namespace ShradhaGeneralBookStore.Areas.Admin.Controllers
{
    public class OrderBackController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
