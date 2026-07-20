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
            modelBuilder.Entity<Product>()
                        .Property(p => p.Price)
                        .HasPrecision(18, 2);
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Laptop", Description = "High-end gaming laptop", Price = 1200, Stock = 10, ImageUrl = "assets/products/laptop.jpg" },
                new Product { Id = 2, Name = "Smartphone", Description = "Latest model smartphone with 5G", Price = 800, Stock = 20, ImageUrl = "assets/products/phone.jpg" },
                new Product { Id = 3, Name = "Headphones", Description = "Noise-cancelling over-ear headphones", Price = 199, Stock = 15, ImageUrl = "assets/products/headphone.jpg" }
            );

            modelBuilder.Entity<Permission>().HasData(
                new Permission { Id = 1, Name = "productView" },
                new Permission { Id = 2, Name = "productCreate" },
                new Permission { Id = 3, Name = "productModify" },
                new Permission { Id = 4, Name = "productDelete" }
            );

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "user" },
                new Role { Id = 2, Name = "admin" }
            );

            modelBuilder.Entity<PermissionRole>().HasData(
                new PermissionRole { PermissionId = 1, RoleId = 1 },
                new PermissionRole { PermissionId = 1, RoleId = 2 },
                new PermissionRole { PermissionId = 2, RoleId = 2 },
                new PermissionRole { PermissionId = 3, RoleId = 2 },
                new PermissionRole { PermissionId = 4, RoleId = 2 }
            );
        }
    }
}
