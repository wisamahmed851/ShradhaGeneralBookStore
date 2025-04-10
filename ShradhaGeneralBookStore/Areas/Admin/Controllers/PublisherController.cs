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
    }
}
