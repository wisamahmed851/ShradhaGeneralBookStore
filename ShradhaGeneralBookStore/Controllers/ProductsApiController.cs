using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShradhaGeneralBookStore.Datas;
using ShradhaGeneralBookStore.Dtos;

namespace ShradhaGeneralBookStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsApiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActiveProducts()
        {
            var products = await _context.Product
                .Include(p => p.Category)
                .Include(p => p.Subcategory)
                .Where(p => p.status == "Active")
                .Select(p => new ProductDto
                {
                    Name = p.Name,

                    CategoryName = p.Category.Name,
                    SubcategoryName = p.Subcategory.Name,
                    Price = p.Price
                })
                .ToListAsync();

            return Ok(products);
        }
    }
    
}
