using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShradhaGeneralBookStore.Datas;

namespace ShradhaGeneralBookStore.Controllers
{
    public class PagesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PagesController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult AboutUs()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        public async Task<IActionResult> Faqs()
        {
            var categories = await _context.FaqCategories.Where(x => x.IsActive).ToListAsync();
            var firstCategoryId = categories.FirstOrDefault()?.Id;

            var faqs = await _context.Faqs
                .Where(f => f.CategoryId == firstCategoryId && f.IsActive)
                .ToListAsync();

            ViewBag.Categories = categories;
            ViewBag.Faqs = faqs;
            ViewBag.SelectedCategoryId = firstCategoryId;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetFaqsByCategory(int categoryId)
        {
            var faqs = await _context.Faqs
                .Where(f => f.CategoryId == categoryId && f.IsActive)
                .ToListAsync();

            return PartialView("_FaqListPartial", faqs);
        }


        public IActionResult Contact()
        {
            return View();
        }
    }
}
