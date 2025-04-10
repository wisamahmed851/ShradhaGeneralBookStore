﻿using Microsoft.AspNetCore.Mvc;
using ShradhaGeneralBookStore.Datas;
using ShradhaGeneralBookStore.Models.Entities;
using ShradhaGeneralBookStore.Filters;
using ShradhaGeneralBookStore.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ShradhaGeneralBookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthorizeAdmin]
    public class AuthorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthorController(ApplicationDbContext context)
        {
            _context = context;
        }


        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
public async Task<IActionResult> Create(Author model)
{
    if (!ModelState.IsValid)
    {
        // Log all validation errors to the Output window
        foreach (var entry in ModelState)
        {
            string key = entry.Key;
            var errors = entry.Value.Errors;

            foreach (var error in errors)
            {
                System.Diagnostics.Debug.WriteLine($"Validation error in '{key}': {error.ErrorMessage}");
            }
        }

        return View(model); // Show validation messages in the view
    }

    await _context.Author.AddAsync(model);
    await _context.SaveChangesAsync();

    TempData["SuccessMessage"] = "Author added successfully!";
    return RedirectToAction("Index");
}



        public async Task<IActionResult> Index()
        {
            var author = await _context.Author.ToListAsync();

            return View(author);
        }
        public async Task<IActionResult> Edit(int id)
        {
            var author = await _context.Author.FindAsync(id);

            return View(author);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Author model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existingAuthor = await _context.Author.FindAsync(model.Id);
            if (existingAuthor == null)
            {
                return NotFound();
            }

            existingAuthor.Name = model.Name;

            _context.Author.Update(existingAuthor);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Author updated successfully!";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var author = await _context.Author.FindAsync(id);
            if (author == null) return NotFound();

            _context.Author.Remove(author);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Author deleted successfully!";
            return RedirectToAction("Index");
        }


    }
}
