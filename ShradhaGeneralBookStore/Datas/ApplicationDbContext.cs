using Microsoft.EntityFrameworkCore;
using ShradhaGeneralBookStore.Models.Entities;

namespace ShradhaGeneralBookStore.Datas
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> User { get; set; }
        public DbSet<Category> Categorie { get; set; }
        public DbSet<Subcategory> Subcategorie { get; set; }
        public DbSet<Author> Author { get; set; }
        public DbSet<Publisher> Publisher { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<ProductImage> ProductImage { get; set; }

        public DbSet<Cart> Cart { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Always call this first or last (doesn't matter)

            // Rename table for User explicitly (optional but helpful)
            modelBuilder.Entity<User>().ToTable("User");

            // Seed Admin User
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                FullName = "Admin",
                Email = "admin@gmail.com",
                Password = "AQAAAAIAAYagAAAAEM+fExcaDSugZ1EnuJv9N/cmcMLvPwQ7JBrH/3aSd5gBqUyENfh4hWXufEYCWhGyhg==", // Hash in real apps
                Phone = null,
                Address = null,
                Role = "Admin",
                IsActive = true,
                CreatedAt = new DateTime(2024, 1, 1)
            });

            // Prevent multiple cascade delete paths for Product relationships
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); // Don’t cascade delete when category is deleted

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Subcategory)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.SubcategoryId)
                .OnDelete(DeleteBehavior.Restrict); // Don’t cascade delete when subcategory is deleted
        }


    }
}
