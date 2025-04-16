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
        public async Task<IActionResult> Index(string searchTerm, int? categoryId, int? subcategoryId, decimal? minPrice, decimal? maxPrice)
            {
                var productsQuery = _context.Product
                    .Include(p => p.ProductImages)
                    .Include(p => p.Category)
                    .Include(p => p.Subcategory)
                    .AsQueryable();

                var subcategories = _context.Subcategorie.ToList();

                var subByCat = subcategories
                    .GroupBy(s => s.CategoryId)
                    .ToDictionary(g => g.Key, g => g.ToList());

                if (!string.IsNullOrEmpty(searchTerm))
                    productsQuery = productsQuery.Where(p => p.Name.Contains(searchTerm));

                if (categoryId.HasValue)
                    productsQuery = productsQuery.Where(p => p.CategoryId == categoryId.Value);

                if (subcategoryId.HasValue)
                    productsQuery = productsQuery.Where(p => p.SubcategoryId == subcategoryId.Value);

                if (minPrice.HasValue)
                    productsQuery = productsQuery.Where(p => p.Price >= minPrice.Value);

                if (maxPrice.HasValue)
                    productsQuery = productsQuery.Where(p => p.Price <= maxPrice.Value);

                var viewModel = new ShopViewModel
                {
                    Categories = await _context.Categorie.ToListAsync(),
                    Subcategories = subcategories,
                    Products = await productsQuery.ToListAsync(),
                    SubcategoriesByCategory = subByCat,
                    SearchTerm = searchTerm,
                    SelectedCategoryId = categoryId,
                    MinPrice = minPrice,
                    MaxPrice = maxPrice
                };

                // 👇 Check if request is AJAX
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    // Return only the product partial
                    return PartialView("_ProductListPartial", viewModel);
                }

                // Otherwise, return full view
                return View(viewModel);
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
