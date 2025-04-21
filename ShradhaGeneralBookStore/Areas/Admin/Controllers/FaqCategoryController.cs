using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShradhaGeneralBookStore.Datas;
using ShradhaGeneralBookStore.Models.Entities;
using ShradhaGeneralBookStore.Models.ViewModel;

namespace ShradhaGeneralBookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FaqCategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FaqCategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var FaqCategories = await _context.FaqCategories.ToListAsync();
            return View(FaqCategories); // Pass data to view
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddFaqsCategoryViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var faqCategory = new FaqCategory
            {
                Name = model.Name
            };

            _context.FaqCategories.Add(faqCategory);
            await _context.SaveChangesAsync(); // Await the async save

            TempData["Success"] = "FAQ Category Created Successfully";
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var category = await _context.FaqCategories.FindAsync(id);
            if (category != null)
            {
                category.IsActive = !category.IsActive;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

    }
}
