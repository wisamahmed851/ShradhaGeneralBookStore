using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShradhaGeneralBookStore.Datas;

namespace ShradhaGeneralBookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FeedBackController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FeedBackController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            // Fetch feedbacks from the database
            var feedbacks = await _context.FeedBacks.ToListAsync();
            return View(feedbacks);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var Feedbcak = await _context.FeedBacks.FindAsync(id);
            if (Feedbcak == null)
            {
                TempData["ErrorMessage"] = "Feedback not found.";
                return RedirectToAction("Index");
            }
            _context.FeedBacks.Remove(Feedbcak);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Feedback deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}
