using Microsoft.AspNetCore.Mvc;
using ShradhaGeneralBookStore.Datas;
using ShradhaGeneralBookStore.Models.Entities;
using Microsoft.EntityFrameworkCore;
using ShradhaGeneralBookStore.Filters;
using System.Threading.Tasks;
using ShradhaGeneralBookStore.Models.ViewModel;

namespace ShradhaGeneralBookStore.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        [ServiceFilter(typeof(AuthorizeUserAttribute))]
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Index", "Home");

            var cartItems = await _context.Cart
                .Include(c => c.Product)
                    .ThenInclude(p => p.ProductImages)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == userId.ToString())
                .ToListAsync();

            var vm = new CartPageViewModel
            {
                CartItems = cartItems,
                Orders = orders
            };

            return View(vm);
        }


        [HttpGet]
        [ServiceFilter(typeof(AuthorizeUserAttribute))]
        public IActionResult AddToCart(int productId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Index", "Home", new { showLogin = true });
            if (productId == null)
                return RedirectToAction("Index", "Home");
            // Check if product already in cart
            var existing = _context.Cart.FirstOrDefault(c => c.UserId == userId && c.ProductId == productId);

            if (existing != null)
            {
                existing.Quantity += 1;
                _context.Cart.Update(existing);
            }
            else
            {
                var cart = new Cart
                {
                    ProductId = productId,
                    Quantity = 1,
                    UserId = userId.Value,
                    AddedAt = DateTime.Now
                };

                _context.Cart.Add(cart);
            }

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Remove(int id)
        {
            var cartItem = await _context.Cart.FindAsync(id);

            if(cartItem == null)
            {
                return NotFound();
            }

            _context.Cart.Remove(cartItem);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Item removed from cart successfully.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int cartId, int quantity)
        {
            if (quantity < 1)
                return BadRequest("Quantity must be at least 1.");

            var cartItem = await _context.Cart.Include(c => c.Product).FirstOrDefaultAsync(c => c.Id == cartId);
            if (cartItem == null)
                return NotFound("Cart item not found.");

            if (cartItem.Product.Stock < quantity)
                return BadRequest("Requested quantity exceeds available stock.");

            cartItem.Quantity = quantity;
            await _context.SaveChangesAsync();

            var itemSubtotal = cartItem.Quantity * cartItem.Product.Price;
            var cartTotal = await _context.Cart
                .Where(c => c.UserId == cartItem.UserId)
                .SumAsync(c => c.Quantity * c.Product.Price);

            return Json(new { itemSubtotal, cartTotal });
        }

    }
}
