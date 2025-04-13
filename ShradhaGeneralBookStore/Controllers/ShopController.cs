using Microsoft.AspNetCore.Mvc;

namespace ShradhaGeneralBookStore.Controllers
{
    public class ShopController : Controller
    {
        public IActionResult index()
        {
            return View();
        }

        public IActionResult Details()
        {
            return View();
        }

        public IActionResult Cart()
        {
            return View();
        }
        public IActionResult Wishlist()
        {
            return View();
        }
    }
}
