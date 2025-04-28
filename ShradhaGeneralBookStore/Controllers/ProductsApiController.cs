using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShradhaGeneralBookStore.Datas;

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
                .Where(p => p.status == "Active")
                .ToListAsync();

            return Ok(products);
        }
    }
    
}
