using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShradhaGeneralBookStore.Datas;
using ShradhaGeneralBookStore.Models;
using ShradhaGeneralBookStore.Models.Entities;
using ShradhaGeneralBookStore.Models.ViewModel;
using ShradhaGeneralBookStore.Models.ViewModels;

namespace ShradhaGeneralBookStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // All products including their related data
            var allProducts = await _context.Product
                .Include(p => p.ProductImages)
                .Include(p => p.Author)
                .Include(p => p.Category)
                .OrderByDescending(p => p.CreatedAt) // assuming you have CreatedAt
                .ToListAsync();

            // Only keep cover images
            foreach (var product in allProducts)
            {
                product.ProductImages = product.ProductImages
                    .Where(img => img.ImageType == ProductImageType.Cover)
                    .ToList();
            }

            // Top category names (static for now)
            var topCategoryNames = new List<string> { "Programming", "Bestsellers", "Science" };

            // Products for top categories
            var topCategoryProducts = allProducts
                .Where(p => topCategoryNames.Contains(p.Category.Name))
                .ToList();

             //Top category stats
            var topCategories = allProducts
                .Where(p => topCategoryNames.Contains(p.Category.Name))
                .GroupBy(p => p.Category.Name)
                .Select(g => new TopCategoryViewModel
                {
                    CategoryName = g.Key,
                    ProductCount = g.Count(),
                    ImageUrl = g.FirstOrDefault()?.ProductImages.FirstOrDefault()?.ImageUrl
                })
                .ToList();

            // Recently added
            var recentlyAdded = allProducts
                .OrderByDescending(p => p.CreatedAt) 
                .Take(6)
                .ToList();

            // sasti books 
            var cheapestProducts = allProducts
                .OrderBy(p => p.Price) 
                .Take(3)
                .ToList();
            
            // Send to ViewModel
            var viewModel = new HomePageViewModel
            {
                RecentlyAddedProducts = recentlyAdded,
                CheapestProducts = cheapestProducts,
                TopCategoryProducts = topCategoryProducts,
                TopCategories = topCategories
            };

            return View(viewModel);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
