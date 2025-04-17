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
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Index", "Home");

            // Always fetch CartItems before anything else
            model.CartItems = await _context.Cart
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            model.TotalAmount = model.CartItems.Sum(c => c.Quantity * c.Product.Price);

            
            var userIdValue = HttpContext.Session.GetInt32("UserId");
            if (userIdValue == null)
                return RedirectToAction("Index", "Home");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
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
                await _context.SaveChangesAsync(); // Save to get order.Id

                var cartItems = await _context.Cart
                    .Include(c => c.Product)
                    .Where(c => c.UserId == userIdValue)
                    .ToListAsync();

                foreach (var item in cartItems)
                {
                    var product = await _context.Product.FindAsync(item.ProductId);
                    if (product == null)
                    {
                        ModelState.AddModelError("", $"Product with ID {item.ProductId} not found.");
                        model.CartItems = cartItems;
                        return View("Index");
                    }

                    if (product.Stock < item.Quantity)
                    {
                        ModelState.AddModelError("", $"Not enough stock for product: {product.Name}");
                        model.CartItems = cartItems;
                        return View("Index");
                    }

                    var orderItem = new OrderItem
                    {
                        OrderId = order.Id,
                        ProductId = item.ProductId,
                        ProductName = item.Product.Name,
                        Quantity = item.Quantity,
                        UnitPrice = item.Product.Price,
                        TotalPrice = item.Product.Price * item.Quantity
                    };

                    _context.OrderItems.Add(orderItem);

                    product.Stock -= item.Quantity;
                    _context.Product.Update(product);
                }

                _context.Cart.RemoveRange(cartItems);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return RedirectToAction("ThankYou");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                // Log the error here if needed
                ModelState.AddModelError("", "An error occurred while placing your order. Please try again.");
                model.CartItems = await _context.Cart
                    .Include(c => c.Product)
                    .Where(c => c.UserId == userIdValue)
                    .ToListAsync();
                return View("Index", model);
            }
        }

    }
}
