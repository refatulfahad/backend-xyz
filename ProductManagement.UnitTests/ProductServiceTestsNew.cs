using AutoMapper.Configuration.Annotations;
using FluentAssertions;
using NSubstitute;
using ProductManagement.Models;
using ProductManagement.Repositories;
using ProductManagement.Services;

namespace ProductManagement.UnitTests
{
    public class ProductServiceTestsNew
    {
        private static readonly Product product1 = new Product { Id = 1, Name = "Laptop", Price = 100 };
        private static readonly Product product2 = new Product { Id = 2, Name = "Television", Price = 200 };
        private readonly IProductRepository _mockProductRepository;
        private readonly ProductService _productService;

        public ProductServiceTestsNew()
        {
            _mockProductRepository = Substitute.For<IProductRepository>();
            _productService = new ProductService(_mockProductRepository);
        }

        [Fact]
        public async Task GetAllProductsAsync_ShouldReturnProducts()
        {
            // Arrange
            var products = new List<Product> { product1, product2 };
            
            _mockProductRepository.GetAllProductsAsync().Returns(products);

            // Act
            var result = await _productService.GetAllProductsAsync();

            // Assert
            result.Should().BeEquivalentTo(products);
        }

        [Fact]
        public async Task GetProductByIdAsync_ShouldReturnProduct_WhenProductExists()
        {
            // Arrange
            _mockProductRepository.GetProductByIdAsync(Arg.Is<int>(product1.Id)).Returns(product1);

            // Act
            var result = await _productService.GetProductByIdAsync(product1.Id);

            // Assert
            result.Should().BeEquivalentTo(product1);
        }

        [Fact]
        public async Task CreateProductAsync_ShouldAddProduct()
        {
            // Arrange
            _mockProductRepository.CreateProductAsync(Arg.Is<Product>(p => p.Id == product1.Id)).Returns(product1);

            // Act
            var result = await _productService.CreateProductAsync(product1);

            // Assert
            result.Should().BeEquivalentTo(product1);
        }

        [Fact]
        public async Task UpdateProductAsync_ShouldUpdateProduct()
        {
            // Arrange
            _mockProductRepository.UpdateProductAsync(Arg.Is<Product>(p => p.Id == product1.Id)).Returns(product1);

            // Act
            var result = await _productService.UpdateProductAsync(product1);

            // Assert
            result.Should().BeEquivalentTo(product1);
        }

        [Fact]
        public async Task DeleteProductAsync_ShouldRemoveProduct()
        {
            // Arrange
            _mockProductRepository.DeleteProductAsync(Arg.Is<int>(product1.Id)).Returns(Task.CompletedTask);

            // Act
            await _productService.DeleteProductAsync(product1.Id);

            // Assert
            _mockProductRepository.Received(1).Invoking(x => x.DeleteProductAsync(1));
        }
    }
}
