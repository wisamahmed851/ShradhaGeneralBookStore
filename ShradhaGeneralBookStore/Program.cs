using Microsoft.EntityFrameworkCore;
using ShradhaGeneralBookStore.Datas;
using ShradhaGeneralBookStore.Filters;
using ShradhaGeneralBookStore.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BookStorePortal")));

builder.Services.AddSession();

builder.Services.AddControllersWithViews()
    .AddSessionStateTempDataProvider();
// Register the custom filter here 👇
builder.Services.AddScoped<AuthorizeUserAttribute>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseRouting();

app.UseSession(); 

app.UseMiddleware<AdminAuthMiddleware>();

app.UseAuthorization();


// Comment out custom methods temporarily if unsure
// app.MapStaticAssets();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseStaticFiles();
app.Run();
