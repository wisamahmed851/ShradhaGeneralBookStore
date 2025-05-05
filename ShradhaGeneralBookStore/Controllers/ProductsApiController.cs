using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShradhaGeneralBookStore.Datas;
using ShradhaGeneralBookStore.Dtos;
using ShradhaGeneralBookStore.Models.Entities;

namespace ShradhaGeneralBookStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsApiController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        public ProductsApiController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActiveProducts()
        {
            var products = await _context.Product
                .Include(p => p.Category)
                .Include(p => p.Subcategory)
                .Include(p => p.ProductImages)
                .Where(p => p.status == "Active")
                .ToListAsync();
            var productDtos = products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                CategoryName = p.Category.Name,
                SubcategoryName = p.Subcategory.Name,
                Price = p.Price,
                ImagePath = p.ProductImages
                             .FirstOrDefault(img => img.ImageType == ProductImageType.Cover)
                             ?.ImageUrl
            }).ToList();

            return Ok(productDtos);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductsDetails(int id)
        {
            var product = await _context.Product
                .Include(p => p.Category)
                .Include(p => p.Subcategory)
                .Include(p => p.Author)
                .Include(p => p.Publisher)
                .Include(p => p.ProductImages)
                .Where(p => p.Id == id)
                .Where(p => p.status == "Active")
                .FirstOrDefaultAsync();
            if (product == null)
                return NotFound(new { message = "Product Not found" });

            var productDto = new ProductDetailsDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                CategoryName = product.Category.Name,
                Subcategoryname = product.Subcategory.Name,
                AuthorName = product.Author.Name,
                PublisherName = product.Publisher.Name,
                Price = product.Price,
                ReleaseDate = product.ReleaseDate,
                Version = product.Version,
                Stock = product.Stock,
                status = product.status,
                CoverImageUrl = product.ProductImages
                    .FirstOrDefault(p => p.ImageType == ProductImageType.Cover)?.ImageUrl,
                DetailImageUrls = product.ProductImages
                .Where(p => p.ImageType == ProductImageType.Detail)
                .Select(p => p.ImageUrl)
                .ToList()
            };

            return Ok(productDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var subcategory = await _context.Subcategorie.FirstOrDefaultAsync(s => s.Id == dto.SubcategoryId);
            if (subcategory == null)
                return NotFound(new { message = "Subcategory not found" });

            dto.CategoryId = subcategory.CategoryId;
            var baseCode = subcategory.Code.Length >= 5 ? subcategory.Code.Substring(0, 5) : subcategory.Code;
            var random = new Random();
            var generatedCode = $"{baseCode.ToUpper()}-{dto.Name[0].ToString().ToUpper()}{random.Next(10, 99)}";

            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                CategoryId = dto.CategoryId,
                SubcategoryId = dto.SubcategoryId,
                AuthorId = dto.AuthorId,
                PublisherId = dto.PublisherId,
                Price = dto.Price,
                ReleaseDate = dto.ReleaseDate,
                Version = dto.Version,
                ProductType = dto.ProductType,
                UniqueCode = generatedCode,
                Stock = dto.Stock,
                status = "Active"
            };

            _context.Product.Add(product);
            await _context.SaveChangesAsync();

            string uploadPath = Path.Combine(_hostEnvironment.WebRootPath, "ProductImages");
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            //if (dto.coverImage != null)
            //{
            //    var coverImageName = $"{Guid.NewGuid()}_{dto.coverImage.FileName}";
            //    var coverImagePath = Path.Combine(uploadPath, coverImageName);
            //    using (var stream = new FileStream(coverImagePath, FileMode.Create))
            //    {
            //        await dto.coverImage.CopyToAsync(stream);
            //    }

            //    var productImage = new ProductImage
            //    {
            //        ImageUrl = $"/ProductImages/{coverImageName}",
            //        ImageType = ProductImageType.Cover,
            //        ProductId = product.Id
            //    };
            //    _context.ProductImage.Add(productImage);

            //    if (dto.detailImages != null)
            //    {
            //        foreach (var detailImage in dto.detailImages)
            //        {
            //            var detailImageName = $"{Guid.NewGuid()}_{detailImage.FileName}";
            //            var detailImagePath = Path.Combine(uploadPath, detailImageName);
            //            using (var stream = new FileStream(detailImagePath, FileMode.Create))
            //            {
            //                await detailImage.CopyToAsync(stream);
            //            }
            //            var productDetailImage = new ProductImage
            //            {
            //                ImageUrl = $"/ProductImages/{detailImageName}",
            //                ImageType = ProductImageType.Detail,
            //                ProductId = product.Id
            //            };
            //            _context.ProductImage.Add(productDetailImage);
            //        }
            //    }
            await _context.SaveChangesAsync();

            return Ok(new { message = "Product created successfully", productId = product.Id });
        }
        }
    }
    


