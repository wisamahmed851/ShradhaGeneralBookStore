using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShradhaGeneralBookStore.Datas;
using ShradhaGeneralBookStore.Filters;
using ShradhaGeneralBookStore.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BookStorePortal")));

builder.Services.AddSession();

builder.Services.AddControllersWithViews()
    .AddSessionStateTempDataProvider();

// Register the custom filter
builder.Services.AddScoped<AuthorizeUserAttribute>();

// ✅ Correct: Add Authentication BEFORE app.Build()
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontendLocalhost", policy =>
    {
        policy.WithOrigins("http://127.0.0.1:5500") // frontend origin
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // 🟢 should be before routing

app.UseRouting();
app.UseCors("AllowFrontendLocalhost"); // 👈 allow CORS here

app.UseAuthentication(); // 🟢 must be before Authorization
app.UseAuthorization();

app.UseSession();

app.UseMiddleware<AdminAuthMiddleware>();

// MVC Routes
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllers();
app.Run();
