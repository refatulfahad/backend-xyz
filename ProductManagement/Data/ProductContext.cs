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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Laptop", Description = "High-end gaming laptop", Price = 1200, Stock = 10 },
                new Product { Id = 2, Name = "Smartphone", Description = "Latest model smartphone with 5G", Price = 800, Stock = 20 },
                new Product { Id = 3, Name = "Headphones", Description = "Noise-cancelling over-ear headphones", Price = 199, Stock = 15 }
            );
        }
    }
}
