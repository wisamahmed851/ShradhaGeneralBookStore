using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShradhaGeneralBookStore.Datas;
using ShradhaGeneralBookStore.Filters;
using ShradhaGeneralBookStore.Models.Entities;
using ShradhaGeneralBookStore.Models.ViewModels;

namespace ShradhaGeneralBookStore.Controllers
{

    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Register(AddUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_AddUserModal", model); 
            }

            var user = new User
            {
                FullName = model.FullName,
                Email = model.Email,
                Phone = model.Phone,
                Address = model.Address,
                //Role an other are default set
            };

            var hasher = new PasswordHasher<User>();
            user.Password = hasher.HashPassword(user, model.Password);

            try
            {
                _context.User.Add(user);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Something went wrong while saving user.");
                return View(model);
            }

            TempData["ShowLoginModal"] = true;

            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, bool rememberMe)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                ViewBag.Error = "Invalid login credentials";
                return View();
            }

            var hasher = new PasswordHasher<User>();

            var result = hasher.VerifyHashedPassword(user, user.Password, password);

            if (result == PasswordVerificationResult.Failed)
            {
                // Fallback: User has plain text password (pre-hashing accounts)
                if (user.Password == password)
                {
                    // Re-hash and update the password in DB
                    user.Password = hasher.HashPassword(user, password);
                    _context.User.Update(user);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    ViewBag.Error = "Invalid login credentials";
                    return View();
                }
            }

            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserRole", user.Role);

            return user.Role == "Admin"
                ? RedirectToAction("Index", "Dashboard", new { area = "Admin" })
                : RedirectToAction("Index", "Home");
        }


        //[HttpPost]
        //public async Task<IActionResult> Login(string email, string password, bool rememberMe)
        //{
        //    var user = await _context.User
        //        .FirstOrDefaultAsync(u => u.Email == email && u.Password == password);

        //    if (user == null)
        //    {
        //        ViewBag.Error = "Invalid login credentials";
        //        return View();
        //    }

        //    HttpContext.Session.SetInt32("UserId", user.Id);
        //    HttpContext.Session.SetString("UserRole", user.Role);

        //    return user.Role == "Admin"
        //        ? RedirectToAction("Index", "Dashboard", new { area = "Admin" })
        //        : RedirectToAction("Index", "Home");
        //}

        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Clears all session data
            return RedirectToAction("Index", "Home"); // Redirect to home or login page
        }
    }
}
