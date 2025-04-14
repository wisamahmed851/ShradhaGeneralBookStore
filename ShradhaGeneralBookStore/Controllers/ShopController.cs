using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShradhaGeneralBookStore.Datas;
using ShradhaGeneralBookStore.Models.Entities;
using ShradhaGeneralBookStore.Models.ViewModel;

namespace ShradhaGeneralBookStore.Controllers
{
    public class ShopController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        public ShopController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }
        public IActionResult index()
        {
            return View();
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Product
                .Include(p => p.Category)
                .Include(p => p.Subcategory)
                .Include(p => p.Author)
                .Include(p => p.Publisher)
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound();

            var viewModel = new ProductDetailsViewModel
            {
                Product = product,
                CoverImageUrl = product.ProductImages
                    .FirstOrDefault(p => p.ImageType == ProductImageType.Cover)?.ImageUrl,
                DetailImageUrls = product.ProductImages
                    .Where(p => p.ImageType == ProductImageType.Detail)
                    .Select(p => p.ImageUrl)
                    .ToList()
            };

            return View(viewModel);
        }


        public IActionResult Cart()
        {
            return View();
        }
        public IActionResult Wishlist()
        {
            return View();
        }
    }
}
