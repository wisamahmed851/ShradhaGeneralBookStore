using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using ShradhaGeneralBookStore.Datas;
using ShradhaGeneralBookStore.Dtos;
using ShradhaGeneralBookStore.Models.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ShradhaGeneralBookStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthApiController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        public AuthApiController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser = await _context.User.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (existingUser != null)
            {
                return BadRequest(new { message = "Email already exists." });
            }


            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address,
            };

            var hasher = new PasswordHasher<User>();
            user.Password = hasher.HashPassword(user, dto.Password);

            await _context.User.AddAsync(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User Created Successfull", userId = user.Id });
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid Email or Password." });
            }
            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.Password, dto.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                return Unauthorized(new { message = "Password is Incorrect." });
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role ?? "User")
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                  issuer: _config["Jwt:Issuer"],
                  audience: _config["Jwt:Audience"],
                  claims: claims,
                  expires: DateTime.Now.AddMinutes(Convert.ToDouble(_config["Jwt:ExpiresInMinutes"])),
                  signingCredentials: creds);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new { token = tokenString });
        
        }
    }
}
