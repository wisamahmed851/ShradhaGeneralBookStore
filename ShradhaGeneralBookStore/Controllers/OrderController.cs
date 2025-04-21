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
            var userIdValue = HttpContext.Session.GetInt32("UserId");
            if (userIdValue == null)
                return RedirectToAction("Index", "Home");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Step 1: Calculate Delivery Charges based on Area
                decimal deliveryCharge = 0;
                switch (model.Area)
                {
                    case "1": // North Nazimabad
                        deliveryCharge = 100;
                        break;
                    case "2": // Nazimabad
                        deliveryCharge = 80;
                        break;
                    case "3": // Gohar
                        deliveryCharge = 120;
                        break;
                    case "4": // Gulshan
                        deliveryCharge = 150;
                        break;
                    case "5": // Malir
                        deliveryCharge = 200;
                        break;
                    case "6": // Korangi
                        deliveryCharge = 180;
                        break;
                    case "7": // Landhi
                        deliveryCharge = 170;
                        break;
                    default:
                        deliveryCharge = 100; // default fallback
                        break;
                }

                model.DeliveryCharge = deliveryCharge;
                model.TotalAmount += deliveryCharge;

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

                TempData["SuccessMessage"] = "Order Placed Successfully!";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError("", "An error occurred while placing your order. Please try again.");
                model.CartItems = await _context.Cart
                    .Include(c => c.Product)
                    .Where(c => c.UserId == userIdValue)
                    .ToListAsync();
                return View("Index", model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> OrderTrack(string OrderNumber)
        {
            var userIdValue = HttpContext.Session.GetInt32("UserId");

            if (userIdValue == null)
            {
                return Json(new { success = false, message = "You must be logged in to view your orders." });
            }

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.OrderNumber == OrderNumber && o.UserId == userIdValue.ToString());

            if (order == null)
            {
                return Json(new { success = false, message = "Order not found or you do not have permission to view it." });
            }

            var result = new
            {
                success = true,
                orderNumber = order.OrderNumber,
                status = order.Status,
                items = order.OrderItems.Select(i => new
                {
                    name = i.Product.Name,
                    price = i.Product.Price,
                    quantity = i.Quantity,
                    subtotal = i.Quantity * i.Product.Price
                }),
                total = order.OrderItems.Sum(i => i.Quantity * i.Product.Price)
            };

            return Json(result);
        }

    }
}
