using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShradhaGeneralBookStore.Datas;
using ShradhaGeneralBookStore.Models.Entities;
using ShradhaGeneralBookStore.Models.ViewModel;

namespace ShradhaGeneralBookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FaqController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FaqController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var Faqs = await _context.Faqs
                .Include(f => f.Category)
                .ToListAsync();
            return View(Faqs);
        }

        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.FaqCategories, "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AddFaqViewModel Model)
        {
            if (!ModelState.IsValid)
            {
                TempData["WarningMessage"] = "Pleace Fixed the Errors Valiadtion and try again";

                ViewData["CategoryId"] = new SelectList(_context.FaqCategories, "Id", "Name");

                return View();
            }
                
            var FaqCategory = await _context.FaqCategories.FindAsync(Model.CategoryId);
            if (FaqCategory == null)
            {
                TempData["ErrorMessage"] = "Faq Category Not Found";
                ViewData["CategoryId"] = new SelectList(_context.FaqCategories, "Id", "Name");

                return View();
            }

            var Faqs = new Faq
            {
                Question = Model.Question,
                Answer = Model.Answer,
                CategoryId = Model.CategoryId
            };

            await _context.Faqs.AddAsync(Faqs);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Faqs Is Successfully Added";
            return RedirectToAction("Index");
        }
        
        public async Task<IActionResult> Edit(AddFaqViewModel Model)
        {

        }
    }
}
