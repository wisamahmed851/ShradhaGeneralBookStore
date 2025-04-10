using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShradhaGeneralBookStore.Datas;
using ShradhaGeneralBookStore.Models.Entities;
using ShradhaGeneralBookStore.Models.ViewModel;

namespace ShradhaGeneralBookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PublisherController : Controller
    {
        private readonly ApplicationDbContext _context;
        public PublisherController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var publisher = await _context.Publisher.ToListAsync();
            return View(publisher);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AddPublisherViewModel model)
        {

            if (!ModelState.IsValid)
                return View(model);

            var publisher = new Publisher

            {
                Name = model.Name
            };

            await _context.Publisher.AddAsync(publisher);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Publisher added successfully!";
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Edit(int id)
        {
            var publisher = await _context.Publisher.FindAsync(id);

            return View(publisher);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Publisher model)
        {
            var existingPublisher = await _context.Publisher.FindAsync(model.Id);
            if(existingPublisher == null)
            {
                return NotFound();
            }
            existingPublisher.Name = model.Name;

            _context.Publisher.Update(existingPublisher);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Publisher updated successfully!";
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int id)
        {
            var publisher = await _context.Publisher.FindAsync(id);
            if (publisher == null)
            {
                return NotFound();
            }
            _context.Publisher.Remove(publisher);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Publisher deleted successfully!";
            return RedirectToAction("Index");
        }
        }
}
