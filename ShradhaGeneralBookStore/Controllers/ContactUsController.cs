using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShradhaGeneralBookStore.Datas;
using ShradhaGeneralBookStore.Models.ViewModel;

namespace ShradhaGeneralBookStore.Models.Entities
{
    public class ContactUsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContactUsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(AddFeedBackViewModel Model)
        {
            var userID = HttpContext.Session.GetInt32("UserId");
            if (userID == null)
            {
                TempData["warningMessage"] = "You Need To Login First";
                return View();
            }
            if (!ModelState.IsValid)
                return View();

            var feedback = new Feedback
            {

                Name = Model.Name,
                Email = Model.Email,
                Message = Model.Message,
                UserId = userID.ToString(),
                CreatedAt = DateTime.UtcNow,
            };
            _context.FeedBacks.Add(feedback);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Your Feed Back is Sended!";
            return View();
        }
    }
}
