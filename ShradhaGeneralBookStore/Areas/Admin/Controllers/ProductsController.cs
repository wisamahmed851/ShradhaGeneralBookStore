using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using ShradhaGeneralBookStore.Datas;
using ShradhaGeneralBookStore.Models.Entities;
using ShradhaGeneralBookStore.Models.ViewModel;
namespace ShradhaGeneralBookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        public ProductsController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Create()
        {
            var subcategories = await _context.Subcategorie
                .Include(sc => sc.Category)
                .ToListAsync();

            // Group subcategories under their categories
            var grouped = subcategories
                .GroupBy(sc => sc.Category)
                .ToDictionary(
                    g => g.Key.Name,   // Category name
                    g => g.Select(sc => new SelectListItem
                    {
                        Value = sc.Id.ToString(),
                        Text = sc.Name
                    }).ToList()
                );

            ViewBag.GroupedSubcategories = grouped;

            ViewData["AuthorId"] = new SelectList(await _context.Author.ToListAsync(), "Id", "Name");
            ViewData["PublisherId"] = new SelectList(await _context.Publisher.ToListAsync(), "Id", "Name");

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync(); // For ViewData reuse on invalid model
                return View(model);
            }

            // Get subcategory from database to use its code
            var subcategory = await _context.Subcategorie.FirstOrDefaultAsync(s => s.Id == model.SubcategoryId);
            if (subcategory == null)
            {
                ModelState.AddModelError("SubcategoryId", "Invalid Subcategory selected.");
                await LoadDropdownsAsync();
                return View(model);
            }

            // Generate Unique Code
            model.CategoryId = subcategory.CategoryId;
            var baseCode = subcategory.Code.Length >= 5 ? subcategory.Code.Substring(0, 5) : subcategory.Code;
            var random = new Random();
            var generatedCode = $"{baseCode.ToUpper()}-{model.Name[0].ToString().ToUpper()}{random.Next(10, 99)}";
            
            var product = new Product
            {
                Name = model.Name,
                Description = model.Description,
                CategoryId = model.CategoryId,
                SubcategoryId = model.SubcategoryId,
                AuthorId = model.AuthorId,
                PublisherId = model.PublisherId,
                Price = model.Price,
                ReleaseDate = model.ReleaseDate,
                Version = model.Version,
                ProductType = model.ProductType,
                UniqueCode = generatedCode,
                Stock = model.Stock
            };

            _context.Product.Add(product);
            await _context.SaveChangesAsync(); // Save to get ProductId

            // Handle Image Upload
            if (model.ImageFiles != null && model.ImageFiles.Count > 0)

            {
                string uploadPath = Path.Combine(_hostEnvironment.WebRootPath, "ProductImages");

                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                foreach (var image in model.ImageFiles)
                {
                    string uniqueFileName = $"{Guid.NewGuid()}_{image.FileName}";
                    string fullPath = Path.Combine(uploadPath, uniqueFileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    var productImage = new ProductImage
                    {
                        ProductId = product.Id,
                        ImageUrl = "/ProductImages/" + uniqueFileName
                    };

                    _context.ProductImage.Add(productImage);
                }


                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "Product created successfully!";
            return RedirectToAction("Index"); // or wherever your product list is
        }

        private async Task LoadDropdownsAsync()
        {
            var subcategories = await _context.Subcategorie
                .Include(sc => sc.Category)
                .ToListAsync();

            // Group subcategories under their categories
            var grouped = subcategories
                .GroupBy(sc => sc.Category)
                .ToDictionary(
                    g => g.Key.Name,   // Category name
                    g => g.Select(sc => new SelectListItem
                    {
                        Value = sc.Id.ToString(),
                        Text = sc.Name
                    }).ToList()
                );

            ViewBag.GroupedSubcategories = grouped;

            ViewData["AuthorId"] = new SelectList(await _context.Author.ToListAsync(), "Id", "Name");
            ViewData["PublisherId"] = new SelectList(await _context.Publisher.ToListAsync(), "Id", "Name");


        }
    }
}
