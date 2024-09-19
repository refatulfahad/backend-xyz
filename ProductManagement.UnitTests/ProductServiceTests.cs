using FluentAssertions;
using Moq;
using ProductManagement.Models;
using ProductManagement.Repositories;
using ProductManagement.Services;

namespace ProductManagement.UnitTests
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _productService = new ProductService(_mockProductRepository.Object);
        }

        [Fact]
        public async Task GetAllProductsAsync_ShouldReturnProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Product1", Price = 100 },
                new Product { Id = 2, Name = "Product2", Price = 200 }
            };
            _mockProductRepository.Setup(repo => repo.GetAllProductsAsync()).ReturnsAsync(products);

            // Act
            var result = await _productService.GetAllProductsAsync();

            // Assert
            result.Should().BeEquivalentTo(products);
        }

        [Fact]
        public async Task GetProductByIdAsync_ShouldReturnProduct_WhenProductExists()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Product1", Price = 100 };
            _mockProductRepository.Setup(repo => repo.GetProductByIdAsync(1)).ReturnsAsync(product);

            // Act
            var result = await _productService.GetProductByIdAsync(1);

            // Assert
            result.Should().BeEquivalentTo(product);
        }

        [Fact]
        public async Task CreateProductAsync_ShouldAddProduct()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Product1", Price = 100 };
            _mockProductRepository.Setup(repo => repo.CreateProductAsync(product)).ReturnsAsync(product);

            // Act
            var result = await _productService.CreateProductAsync(product);

            // Assert
            result.Should().BeEquivalentTo(product);
        }

        [Fact]
        public async Task UpdateProductAsync_ShouldUpdateProduct()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Product1", Price = 100 };
            _mockProductRepository.Setup(repo => repo.UpdateProductAsync(product)).ReturnsAsync(product);

            // Act
            var result = await _productService.UpdateProductAsync(product);

            // Assert
            result.Should().BeEquivalentTo(product);
        }

        [Fact]
        public async Task DeleteProductAsync_ShouldRemoveProduct()
        {
            // Arrange
            _mockProductRepository.Setup(repo => repo.DeleteProductAsync(1)).Returns(Task.CompletedTask);

            // Act
            await _productService.DeleteProductAsync(1);

            // Assert
            _mockProductRepository.Verify(repo => repo.DeleteProductAsync(1), Times.Once);
        }
    }
}
