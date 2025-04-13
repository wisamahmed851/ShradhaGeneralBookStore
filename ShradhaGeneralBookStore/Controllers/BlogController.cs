using Microsoft.AspNetCore.Mvc;

namespace ShradhaGeneralBookStore.Controllers
{
    public class BlogController : Controller
    {
        public IActionResult Grid()
        {
            return View();
        }

        public IActionResult List()
        {
            return View();
        }

        public IActionResult Detail()
        {
            return View();
        }
    }
}
