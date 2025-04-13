using Microsoft.AspNetCore.Mvc;

namespace ShradhaGeneralBookStore.Controllers
{
    public class PagesController : Controller
    {
        public IActionResult AboutUs()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        public IActionResult Faqs()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }
    }
}
