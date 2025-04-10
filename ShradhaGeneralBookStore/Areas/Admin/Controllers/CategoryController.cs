using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShradhaGeneralBookStore.Datas;
using ShradhaGeneralBookStore.Models.Entities;
using ShradhaGeneralBookStore.Models.ViewModel;

namespace ShradhaGeneralBookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var Categories = await _context.Categorie.ToListAsync();
            return View(Categories);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AddCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Get first two letters of the name, uppercase
                string code = new string(model.Name
                    .Where(char.IsLetter)
                    .Take(2)
                    .ToArray())
                    .ToUpper();

                if (code.Length < 2)
                {
                    ModelState.AddModelError("Name", "Category name must contain at least two letters.");
                    return View(model);
                }

                // Check if the generated code already exists
                bool codeExists = _context.Categorie.Any(c => c.Code == code);
                if (codeExists)
                {
                    ModelState.AddModelError("Name", "A category with this code already exists. Try a different name.");
                    return View(model);
                }

                var category = new Category
                {
                    Name = model.Name,
                    Code = code
                };

                _context.Categorie.Add(category);
                _context.SaveChanges();

                TempData["Success"] = "Category created successfully!";
                return RedirectToAction("Index");
            }

            return View(model);
        }
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _context.Categorie.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            // Only passing the Name since Code is auto-generated and shouldn't be edited
            var viewModel = new AddCategoryViewModel
            {
                Name = category.Name
            };

            ViewBag.CategoryId = category.Id; // for the hidden input
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AddCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var category = await _context.Categorie.FindAsync(id);
                if (category == null)
                {
                    return NotFound();
                }

                string code = new string(model.Name
                    .Where(char.IsLetter)
                    .Take(2)
                    .ToArray())
                    .ToUpper();

                if (code.Length < 2)
                {
                    ModelState.AddModelError("Name", "Category name must contain at least two letters.");
                    return View(model);
                }

                // Check for duplicate code but ignore the current record
                bool codeExists = _context.Categorie.Any(c => c.Code == code && c.Id != id);
                if (codeExists)
                {
                    ModelState.AddModelError("Name", "A category with this code already exists. Try a different name.");
                    return View(model);
                }

                category.Name = model.Name;
                category.Code = code;

                await _context.SaveChangesAsync();
                TempData["Success"] = "Category updated successfully!";
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = id;
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categorie.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            _context.Categorie.Remove(category);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Category deleted successfully!";
            return RedirectToAction("Index");
        }

    }
}
