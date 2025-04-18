using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
            foreach (var product in products)
            {
                product.ProductImages = product.ProductImages
                    .Where(img => img.ImageType == ProductImageType.Cover)
                    .ToList();
            }
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

            string uploadPath = Path.Combine(_hostEnvironment.WebRootPath, "ProductImages");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            // ✅ Save Cover Image
            if (model.coverImage != null)
            {
                string uniqueFileName = $"{Guid.NewGuid()}_{model.coverImage.FileName}";
                string fullPath = Path.Combine(uploadPath, uniqueFileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await model.coverImage.CopyToAsync(stream);
                }

                var coverProductImage = new ProductImage
                {
                    ProductId = product.Id,
                    ImageUrl = "/ProductImages/" + uniqueFileName,
                    ImageType = ProductImageType.Cover
                };

                _context.ProductImage.Add(coverProductImage);
            }

            // ✅ Save Detail Images
            if (model.detailImages != null && model.detailImages.Count > 0)
            {
                foreach (var image in model.detailImages)
                {
                    string uniqueFileName = $"{Guid.NewGuid()}_{image.FileName}";
                    string fullPath = Path.Combine(uploadPath, uniqueFileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    var detailProductImage = new ProductImage
                    {
                        ProductId = product.Id,
                        ImageUrl = "/ProductImages/" + uniqueFileName,
                        ImageType = ProductImageType.Detail
                    };

                    _context.ProductImage.Add(detailProductImage);
                }
            }

            await _context.SaveChangesAsync();


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
            ViewBag.CoverImage = product.ProductImages
                .FirstOrDefault(p => p.ImageType == ProductImageType.Cover)?.ImageUrl;
            ViewBag.DetailImages = product.ProductImages
                .Where(p => p.ImageType == ProductImageType.Detail)
                .Select(p => p.ImageUrl)
                .ToList();

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
            string imagePathRoot = Path.Combine(_hostEnvironment.WebRootPath, "ProductImages");

            // ✅ 1. Handle Cover Image
            if (model.coverImage != null)
            {
                // Delete existing cover image from wwwroot
                var coverImage = product.ProductImages
                    .FirstOrDefault(img => img.ImageType == ProductImageType.Cover);

                if (coverImage != null)
                {
                    string fullPath = Path.Combine(_hostEnvironment.WebRootPath, coverImage.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }

                    // Remove from DB
                    _context.ProductImage.Remove(coverImage);
                    await _context.SaveChangesAsync();
                }

                // Upload new cover image
                string uniqueCoverName = $"{Guid.NewGuid()}_{model.coverImage.FileName}";
                string coverFullPath = Path.Combine(imagePathRoot, uniqueCoverName);

                using (var stream = new FileStream(coverFullPath, FileMode.Create))
                {
                    await model.coverImage.CopyToAsync(stream);
                }

                var newCoverImage = new ProductImage
                {
                    ProductId = product.Id,
                    ImageUrl = "/ProductImages/" + uniqueCoverName,
                    ImageType = ProductImageType.Cover
                };
                _context.ProductImage.Add(newCoverImage);
            }

            // ✅ 2. Handle Detail Images
            if (model.detailImages != null && model.detailImages.Count > 0)
            {
                // Delete existing detail images
                var detailImages = product.ProductImages
                    .Where(img => img.ImageType == ProductImageType.Detail)
                    .ToList();

                foreach (var img in detailImages)
                {
                    string fullPath = Path.Combine(_hostEnvironment.WebRootPath, img.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }

                _context.ProductImage.RemoveRange(detailImages);
                await _context.SaveChangesAsync();

                // Upload new detail images
                foreach (var image in model.detailImages)
                {
                    string uniqueFileName = $"{Guid.NewGuid()}_{image.FileName}";
                    string fullPath = Path.Combine(imagePathRoot, uniqueFileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    var newDetailImage = new ProductImage
                    {
                        ProductId = product.Id,
                        ImageUrl = "/ProductImages/" + uniqueFileName,
                        ImageType = ProductImageType.Detail
                    };
                    _context.ProductImage.Add(newDetailImage);
                }
            }

            await _context.SaveChangesAsync();


            TempData["SuccessMessage"] = "Product updated successfully!";
            return RedirectToAction(nameof(Index));
        }

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

            var viewModel = new DetailsProductViewModel
            {
                Name = product.Name,
                Description = product.Description,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name,
                SubcategoryId = product.SubcategoryId,
                SubcategoryName = product.Subcategory?.Name,
                AuthorId = (int)product.AuthorId,
                AuthorName = product.Author?.Name,
                PublisherId = (int)product.PublisherId,
                PublisherName = product.Publisher?.Name,
                Price = product.Price,
                ReleaseDate = product.ReleaseDate,
                Version = product.Version,
                ProductType = product.ProductType,
                Stock = product.Stock,
                CoverImage = product.ProductImages
                    .FirstOrDefault(p => p.ImageType == ProductImageType.Cover)?.ImageUrl?.Replace("~", ""),
                DetailImages = product.ProductImages
                    .Where(p => p.ImageType == ProductImageType.Detail)
                    .Select(p => p.ImageUrl?.Replace("~", ""))
                    .ToList()
            };

            var json = JsonConvert.SerializeObject(viewModel, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            return Content(json, "application/json");
        }


        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Product
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            // Delete images from wwwroot
            foreach (var image in product.ProductImages)
            {
                string fullPath = Path.Combine(_hostEnvironment.WebRootPath, image.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }

            // Remove images from database
            _context.ProductImage.RemoveRange(product.ProductImages);

            // Remove product
            _context.Product.Remove(product);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Product deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

    }
}
