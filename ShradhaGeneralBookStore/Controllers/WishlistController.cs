using Microsoft.AspNetCore.Mvc;
using ShradhaGeneralBookStore.Datas;
using ShradhaGeneralBookStore.Models.Entities;
using Microsoft.EntityFrameworkCore; // for Include, ThenInclude, AnyAsync, etc.
using System.Linq;

namespace ShradhaGeneralBookStore.Controllers
{
    public class WishlistController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WishlistController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Add(int productId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Index", "Home");
            var productExists = await _context.Product.AnyAsync(p => p.Id == productId);
            if (!productExists)
            {
                return Json(new { success = false, message = "Product not found" });
            }
            var exists = await _context.Wishlists
                .AnyAsync(w => w.UserId == userId && w.ProductId == productId);

            if (!exists)
            {
                var wishlist = new Wishlist
                {
                    ProductId = productId,
                    UserId = userId.Value
                };
                _context.Wishlists.Add(wishlist);
                await _context.SaveChangesAsync();
            }

            return Json(new { success = true });
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Index", "Home");

            var wishlistItems = await _context.Wishlists
                .Where(w => w.UserId == userId)
                .Include(w => w.Product)
                    .ThenInclude(p => p.ProductImages)
                .Include(w => w.Product.Author)
                .ToListAsync();

            return View(wishlistItems);
        }
       
    }
}
