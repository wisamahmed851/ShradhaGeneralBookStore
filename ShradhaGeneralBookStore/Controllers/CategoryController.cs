using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShradhaGeneralBookStore.Datas;
using ShradhaGeneralBookStore.Models;
using ShradhaGeneralBookStore.Models.Entities;
using System;
using System.Threading.Tasks;

namespace ShradhaGeneralBookStore.Controllers
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
            var categories = await _context.Categorie.ToListAsync();
            return View(categories);
        }

        // GET: Admin/Category/Create
        public IActionResult Create()
        {
            return View();
        }
        // POST: Admin/Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }
            try
            {
                await _context.Categorie.AddAsync(category);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Category created successfully!";
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "Failed to create category. Try again.");
                // You can also log the exception here
            }
            return RedirectToAction("Index");
        }
        // GET: Admin/Category/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _context.Categorie.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Admin/Category/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            try
            {
                _context.Update(category);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Category updated successfully!";
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "Failed to update category. Try again.");
                // You can also log the exception here
            }

            return RedirectToAction("Index"); // or any page you want to redirect to
        }
    }
}
