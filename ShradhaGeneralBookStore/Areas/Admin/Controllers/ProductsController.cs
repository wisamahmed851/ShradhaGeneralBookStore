using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Newtonsoft.Json;
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

        public async Task<IActionResult> Index()
        {
            var products = await _context.Product
                .Include(p => p.Category)
                .Include(p => p.Subcategory)
                .Include(p => p.Author)
                .Include(p => p.Publisher)
                .Include(p => p.ProductImages)
                .ToListAsync();
            return View(products);
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
                await LoadDropdownsAsync(); 
                return View(model);
            }

            
            var subcategory = await _context.Subcategorie.FirstOrDefaultAsync(s => s.Id == model.SubcategoryId);
            if (subcategory == null)
            {
                ModelState.AddModelError("SubcategoryId", "Invalid Subcategory selected.");
                await LoadDropdownsAsync();
                return View(model);
            }

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
            await _context.SaveChangesAsync();

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
            return RedirectToAction("Index"); 
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

        public async Task<IActionResult> Edit(int id)
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

            // Convert entity to ViewModel
            var viewModel = new EditProductViewModel
            {
                Name = product.Name,
                Description = product.Description,
                CategoryId = product.CategoryId,
                SubcategoryId = product.SubcategoryId,
                AuthorId = product.AuthorId,
                PublisherId = product.PublisherId,
                Price = product.Price,
                ReleaseDate = product.ReleaseDate,
                Version = product.Version,
                ProductType = product.ProductType,
                Stock = product.Stock,
                ImageFiles = new List<IFormFile>() // View expects it, but skip loading files here
            };

            // Dropdowns
            ViewData["AuthorId"] = new SelectList(_context.Author, "Id", "Name", product.AuthorId);
            ViewData["PublisherId"] = new SelectList(_context.Publisher, "Id", "Name", product.PublisherId);

            // Group subcategories by category
            var groupedSubcategories = _context.Subcategorie
                .Include(s => s.Category)
                .ToList()
                .GroupBy(s => s.Category.Name)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(s => new SelectListItem
                    {
                        Value = s.Id.ToString(),
                        Text = s.Name
                    }).ToList()
                );

            ViewBag.GroupedSubcategories = groupedSubcategories;
            ViewBag.ExistingImages = product.ProductImages.Select(p => p.ImageUrl).ToList();

            return View(viewModel); // ✅ Now this matches the View's model
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync();
                return View(model);
            }

            var product = await _context.Product
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            // ✅ Update only changed fields
            if (product.Name != model.Name) product.Name = model.Name;
            if (product.Description != model.Description) product.Description = model.Description;
            if (product.AuthorId != model.AuthorId) product.AuthorId = model.AuthorId;
            if (product.PublisherId != model.PublisherId) product.PublisherId = model.PublisherId;
            if (product.Price != model.Price) product.Price = model.Price;
            if (product.ReleaseDate != model.ReleaseDate) product.ReleaseDate = model.ReleaseDate;
            if (product.Version != model.Version) product.Version = model.Version;
            if (product.ProductType != model.ProductType) product.ProductType = model.ProductType;
            if (product.Stock != model.Stock) product.Stock = model.Stock;
            if (product.SubcategoryId != model.SubcategoryId)
            {
                product.SubcategoryId = model.SubcategoryId;

                // Update CategoryId also
                var subcategory = await _context.Subcategorie.FirstOrDefaultAsync(s => s.Id == model.SubcategoryId);
                if (subcategory != null)
                {
                    product.CategoryId = subcategory.CategoryId;
                }
            }

            // ✅ Handle images (if new ones are uploaded)
            if (model.ImageFiles != null && model.ImageFiles.Count > 0)
            {
                // 1. Delete old images from wwwroot
                string imagePathRoot = Path.Combine(_hostEnvironment.WebRootPath, "ProductImages");
                foreach (var img in product.ProductImages)
                {
                    string fullPath = Path.Combine(_hostEnvironment.WebRootPath, img.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }

                // 2. Remove from DB
                _context.ProductImage.RemoveRange(product.ProductImages);
                await _context.SaveChangesAsync(); // Commit deletion

                // 3. Upload new images
                foreach (var image in model.ImageFiles)
                {
                    string uniqueFileName = $"{Guid.NewGuid()}_{image.FileName}";
                    string fullPath = Path.Combine(imagePathRoot, uniqueFileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    var newImage = new ProductImage
                    {
                        ProductId = product.Id,
                        ImageUrl = "/ProductImages/" + uniqueFileName
                    };
                    _context.ProductImage.Add(newImage);
                }
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Product updated successfully!";
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public IActionResult GetProductDetails(int id)
        {
            var product = _context.Product
                .Include(p => p.Category)
                .Include(p => p.Subcategory)
                .Include(p => p.Author)
                .Include(p => p.Publisher)
                .Include(p => p.ProductImages)
                .FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }
            // Serialize manually to avoid circular reference issues
            var json = JsonConvert.SerializeObject(product, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            return Content(json, "application/json");

        }

    }
}
