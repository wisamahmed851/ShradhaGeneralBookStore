using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShradhaGeneralBookStore.Datas;
using ShradhaGeneralBookStore.Models.Entities;
using ShradhaGeneralBookStore.Models.ViewModel;
using System.Linq;
using System.Threading.Tasks;

namespace ShradhaGeneralBookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SubcategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubcategoryController(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            var subCategorie = await _context.Subcategorie.Include(s => s.Category).ToListAsync();
            return View(subCategorie);
        }



        // GET: Admin/Subcategory/Create
        public IActionResult Create()
        {
            // Populate Categorie for the dropdown
            ViewData["CategoryId"] = new SelectList(_context.Categorie, "Id", "Name");
            return View();
        }

        // POST: Admin/Subcategory/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddSubcategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Generate the code for the subcategory based on the first two letters of the name, uppercase
                string code = new string(model.Name
                    .Where(char.IsLetter)
                    .Take(2)
                    .ToArray())
                    .ToUpper();

                // Ensure the code is 5 characters long by padding with the first 3 characters of the ManufacturerName or a default pattern
                if (code.Length < 5)
                {
                    string manufacturerCode = string.IsNullOrEmpty(model.ManufacturerName) ? "XX" : new string(model.ManufacturerName
                        .Where(char.IsLetter)
                        .Take(3)
                        .ToArray()).ToUpper();
                    code = (code + manufacturerCode).Substring(0, 5); // Truncate to 5 characters if necessary
                }

                // Check if the generated code already exists
                bool codeExists = _context.Subcategorie.Any(s => s.Code == code);
                if (codeExists)
                {
                    ModelState.AddModelError("Name", "A subcategory with this code already exists. Try a different name.");
                    return View(model);
                }

                // Create the Subcategory
                var subcategory = new Subcategory
                {
                    Name = model.Name,
                    CategoryId = model.CategoryId,
                    ManufacturerName = model.ManufacturerName,
                    Code = code
                };

                _context.Subcategorie.Add(subcategory);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Subcategory created successfully!";
                return RedirectToAction("Index");
            }

            // If ModelState is not valid, return the view with the error messages
            ViewData["CategoryId"] = new SelectList(_context.Categorie, "Id", "Name", model.CategoryId);
            return View(model);
        }

        // GET: Admin/Subcategory/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var subcategory = await _context.Subcategorie.FindAsync(id);
            if (subcategory == null)
            {
                return NotFound();
            }

            // Populate Categorie for the dropdown
            ViewData["CategoryId"] = new SelectList(_context.Categorie, "Id", "Name", subcategory.CategoryId);

            // Return the Edit View with the existing subcategory details
            return View(subcategory);
        }

        // POST: Admin/Subcategory/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AddSubcategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Fetch the existing subcategory
                var subcategory = await _context.Subcategorie.FindAsync(id);
                if (subcategory == null)
                {
                    return NotFound();
                }

                // Update properties if changed
                subcategory.Name = model.Name;
                subcategory.CategoryId = model.CategoryId;
                subcategory.ManufacturerName = model.ManufacturerName;

                // Generate the new code based on the new Name and ManufacturerName
                string code = new string(model.Name
                    .Where(char.IsLetter)
                    .Take(2)
                    .ToArray())
                    .ToUpper();

                // Ensure the code is 5 characters long
                if (code.Length < 5)
                {
                    string manufacturerCode = string.IsNullOrEmpty(model.ManufacturerName) ? "XX" : new string(model.ManufacturerName
                        .Where(char.IsLetter)
                        .Take(3)
                        .ToArray()).ToUpper();
                    code = (code + manufacturerCode).Substring(0, 5);
                }

                // Check if the generated code already exists in another subcategory
                bool codeExists = _context.Subcategorie.Any(s => s.Code == code && s.Id != id); // Ensure it's a different subcategory
                if (codeExists)
                {
                    ModelState.AddModelError("Name", "A subcategory with this code already exists. Try a different name.");
                    ViewData["CategoryId"] = new SelectList(_context.Categorie, "Id", "Name", model.CategoryId);
                    return View(model);
                }

                subcategory.Code = code;

                _context.Update(subcategory);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Subcategory updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Categorie, "Id", "Name", model.CategoryId);
            return View(model);
        }

        // GET: Admin/Subcategory/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var subcategory = await _context.Subcategorie
                .Include(s => s.Products) // Include related products to check for dependencies
                .FirstOrDefaultAsync(s => s.Id == id);

            if (subcategory == null)
            {
                return NotFound();
            }

            // Check if any products are using this subcategory
            if (subcategory.Products.Any())
            {
                TempData["ErrorMessage"] = "This subcategory cannot be deleted because it is being used by one or more products.";
                return RedirectToAction(nameof(Index));
            }

            // Proceed with deletion
            _context.Subcategorie.Remove(subcategory);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Subcategory deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        // Other actions for Index, Edit, Delete (if needed)
    }
}
