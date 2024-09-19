using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using ProductManagement.Domain;
using ProductManagement.Models;
using System.Net;
using System.Net.Http.Json;

namespace ProductManagement.IntegrationTests
{
    //implement a previously created WebApplicationFactory class
    //IClassFixture interface is a decorator which indicates that tests in this class rely on a fixture to run
    public class ProductControllerTests : IClassFixture<ProductWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        public ProductControllerTests(ProductWebApplicationFactory<Program> factory) 
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetProducts_ShouldReturnOkResult_WithProductList()
        {
            // Act
            var response = await _client.GetAsync("/product");

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            var products = await response.Content.ReadFromJsonAsync<IEnumerable<ProductDto>>();
            Assert.NotNull(products);
        }

        [Fact]
        public async Task GetProduct_ShouldReturnOkResult_WithProduct()
        {
            // Act
            var response = await _client.GetAsync("/product/2");

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            var product = await response.Content.ReadFromJsonAsync<ProductDto>();
            Assert.NotNull(product);
        }

        [Fact]
        public async Task GetProduct_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            // Act
            var response = await _client.GetAsync("/product/999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CreateProduct_ShouldReturnCreatedProduct()
        {
            // Arrange
            var productDto = new UpsertProductDto { Name = "Television", Description = "LG's smart TV", Price = 8000 };

            // Act
            var response = await _client.PostAsJsonAsync("/product", productDto);

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var createdProduct = await response.Content.ReadFromJsonAsync<ProductDto>();
            Assert.NotNull(createdProduct);
            Assert.Equal(productDto.Name, createdProduct.Name);
            Assert.Equal(productDto.Price, createdProduct.Price);
        }

        [Fact]
        public async Task UpdateProduct_ShouldReturnUpdatedProduct_WhenProductExists()
        {
            // Arrange
            var productDto = new UpsertProductDto { Name = "Television", Description = "LG's smart TV", Price = 8000 };
            var createResponse = await _client.PostAsJsonAsync("/product", productDto);
            createResponse.EnsureSuccessStatusCode();
            var createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductDto>();

            productDto.Name = "Television 2.0";
            productDto.Price = 6000;

            // Act
            var updateResponse = await _client.PutAsJsonAsync($"/product/{createdProduct.Id}", productDto);
            updateResponse.EnsureSuccessStatusCode();
            var updatedProduct = await updateResponse.Content.ReadFromJsonAsync<ProductDto>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
            Assert.NotNull(updatedProduct);
            Assert.Equal(createdProduct.Id, updatedProduct.Id);
            Assert.Equal(productDto.Name, updatedProduct.Name); 
            Assert.Equal(productDto.Price, updatedProduct.Price);
        }

        [Fact]
        public async Task UpdateProduct_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var productDto = new UpsertProductDto { Name = "Non-existent Product", Description = "test description" };

            // Act
            var response = await _client.PutAsJsonAsync($"/product/999", productDto);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteProduct_ShouldReturnNoContent()
        {
            // Arrange
            var productDto = new UpsertProductDto { Name = "Television", Description = "LG's smart TV", Price = 8000 };
            var createResponse = await _client.PostAsJsonAsync("/product", productDto);
            createResponse.EnsureSuccessStatusCode();

            var createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductDto>();

            // Act
            var deleteResponse = await _client.DeleteAsync($"/product/{createdProduct.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        }
    }
}
