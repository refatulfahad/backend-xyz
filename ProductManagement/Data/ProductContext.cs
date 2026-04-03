using Microsoft.EntityFrameworkCore;
using ProductManagement.Models;

namespace ProductManagement.Data
{
    public class ProductContext : DbContext
    {
        public ProductContext(DbContextOptions<ProductContext> options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<PermissionRole> PermissionRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<PermissionRole>().HasKey(sc => new { sc.PermissionId, sc.RoleId });
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Laptop", Description = "High-end gaming laptop", Price = 1200, Stock = 10, ImageUrl = "https://cdn.example.com/images/laptop.jpg" },
                new Product { Id = 2, Name = "Smartphone", Description = "Latest model smartphone with 5G", Price = 800, Stock = 20, ImageUrl = "https://cdn.example.com/images/smartphone.jpg" },
                new Product { Id = 3, Name = "Headphones", Description = "Noise-cancelling over-ear headphones", Price = 199, Stock = 15, ImageUrl = "https://cdn.example.com/images/headphones.jpg" }
            );
        }
    }
}
