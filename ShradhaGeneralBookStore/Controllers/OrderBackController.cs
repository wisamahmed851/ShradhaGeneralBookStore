using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShradhaGeneralBookStore.Datas;
using ShradhaGeneralBookStore.Models.Entities;
using ShradhaGeneralBookStore.Models.ViewModel;

namespace ShradhaGeneralBookStore.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

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

            var viewModel = new CheckoutViewModel
            {
                CartItems = cartItems,
                TotalAmount = cartItems.Sum(c => c.Quantity * c.Product.Price), // Example calc
                DeliveryCharge = 0, // You'll calculate this later based on location maybe
            };

            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // You may want to re-fetch cart items if the model fails validation
                var userId = HttpContext.Session.GetInt32("UserId");
                model.CartItems = await _context.Cart
                    .Include(c => c.Product)
                    .Where(c => c.UserId == userId)
                    .ToListAsync();

                return View("Index", model);
            }

            var userIdValue = HttpContext.Session.GetInt32("UserId");

            var order = new Order
            {
                UserId = userIdValue.ToString(),
                FullName = model.FullName,
                Phone = model.Phone,
                Email = model.Email,
                Address = model.Address,
                Area = model.Area,
                DeliveryCharge = model.DeliveryCharge,
                TotalAmount = model.TotalAmount,
                CreatedAt = DateTime.Now,
                Status = "Pending",
                OrderNumber = "ORD" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Save OrderItems
            var cartItems = await _context.Cart
                .Include(c => c.Product)
                .Where(c => c.UserId == userIdValue)
                .ToListAsync();

            foreach (var item in cartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product.Price,
                    TotalPrice = item.Product.Price * item.Quantity
                };

                _context.OrderItems.Add(orderItem);
            }

            // Remove items from cart
            _context.Cart.RemoveRange(cartItems);

            await _context.SaveChangesAsync();

            return RedirectToAction("ThankYou");
        }

    }
}
