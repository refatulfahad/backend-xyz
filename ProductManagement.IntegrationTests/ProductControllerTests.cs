using ProductManagement.Domain;
using System.Net;
using System.Net.Http.Headers;
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
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICIxam1uaDBwUmwwcWxPakQ4Q04zaFI0b1VicGJqemdrRWgxdWk4U3R1RzlRIn0.eyJleHAiOjE3NDE1MjcxMDIsImlhdCI6MTc0MTUyNjIwMiwiYXV0aF90aW1lIjoxNzQxNTI1NDQ0LCJqdGkiOiJmNjNjMTU3Ny1mOGVjLTQ3NGItYTZhMS0wNGU0MjdlNWJlZGEiLCJpc3MiOiJodHRwOi8vbG9jYWxob3N0OjgwODAvcmVhbG1zL215cmVhbG0xIiwiYXVkIjoiYWNjb3VudCIsInN1YiI6ImMwYWNkMDc1LWU4YjEtNDJhNC1iY2FiLWM0ODdhMGQxNDE3ZiIsInR5cCI6IkJlYXJlciIsImF6cCI6InB1YmxpYy1jbGllbnQiLCJzaWQiOiI3M2M1MWZiNi1iMzM2LTQxZmItODI4NS0xNWQ1YTY0ZjNhNjYiLCJhY3IiOiIwIiwiYWxsb3dlZC1vcmlnaW5zIjpbImh0dHBzOi8vbG9jYWxob3N0OjcyMDAiXSwicmVhbG1fYWNjZXNzIjp7InJvbGVzIjpbImRlZmF1bHQtcm9sZXMtbXlyZWFsbTEiLCJvZmZsaW5lX2FjY2VzcyIsInVtYV9hdXRob3JpemF0aW9uIl19LCJzY29wZSI6Im9wZW5pZCBhdWQgcHJvZmlsZSBVc2VyX1JlYWxtX1JvbGUgZW1haWwiLCJlbWFpbF92ZXJpZmllZCI6dHJ1ZSwicm9sZXMiOlsidXNlciJdLCJuYW1lIjoiUmVmYXR1bCBGYWhhZCIsInByZWZlcnJlZF91c2VybmFtZSI6InJlZmF0QGdtYWlsLmNvbSIsImdpdmVuX25hbWUiOiJSZWZhdHVsIiwiZmFtaWx5X25hbWUiOiJGYWhhZCIsImVtYWlsIjoicmVmYXRAZ21haWwuY29tIn0.Im07ngFgrqJv7bhqnsaD_oQM0C2mj9eQyrOYRIlgTlNnVqRJ7K1z5xRCZm1tTCyVSxBXhVxe5sKaSAFUkup3SijFePS7IwqpsSHTtvWgi5CqC8jWsTEZYFKmjrLvXbDjLro3HwyGkbx9MjPWEETh1hL0NRXnBVlFw7zwhVfXB9Qsz6hTBlGdtgkH0uL_mHCooAib4ZxN4uGrI5UBKi4N-4DHK8NyKfa5pQyvR-yKz-RCrG9Z-E82ldjixuxHT69e4hdrMKYskfznAfQLhi1we3m1Zmx28-qle8vfalhjSUnDQwbx_XvcxqZgHZe-ypCexMkPVSPLbQ5FUStCk3HuIA");
        }

        [Fact]
        public async Task GetProducts_ShouldReturnOkResult_WithProductList()
        {
            // Act
            var response = await _client.GetAsync("/api/product");

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            var products = await response.Content.ReadFromJsonAsync<IEnumerable<ProductDto>>();
            Assert.NotNull(products);
        }

        [Fact]
        public async Task GetProduct_ShouldReturnOkResult_WithProduct()
        {
            // Act
            var response = await _client.GetAsync("/api/product/2");

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            var product = await response.Content.ReadFromJsonAsync<ProductDto>();
            Assert.NotNull(product);
        }

        [Fact]
        public async Task GetProduct_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            // Act
            var response = await _client.GetAsync("/api/product/999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CreateProduct_ShouldReturnCreatedProduct()
        {
            // Arrange
            var productDto = new UpsertProductDto { Name = "Television", Description = "LG's smart TV", Price = 8000 };

            // Act
            var response = await _client.PostAsJsonAsync("/api/product", productDto);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new Exception($"API call failed with status {response.StatusCode}: {errorMessage}");
            }

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
            var createResponse = await _client.PostAsJsonAsync("/api/product", productDto);
            createResponse.EnsureSuccessStatusCode();
            var createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductDto>();

            productDto.Name = "Television 2.0";
            productDto.Price = 6000;

            // Act
            var updateResponse = await _client.PutAsJsonAsync($"/api/product/{createdProduct.Id}", productDto);
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
            var response = await _client.PutAsJsonAsync($"/api/product/999", productDto);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteProduct_ShouldReturnNoContent()
        {
            // Arrange
            var productDto = new UpsertProductDto { Name = "Television", Description = "LG's smart TV", Price = 8000 };
            var createResponse = await _client.PostAsJsonAsync("/api/product", productDto);
            createResponse.EnsureSuccessStatusCode();

            var createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductDto>();

            // Act
            var deleteResponse = await _client.DeleteAsync($"/api/product/{createdProduct.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        }
    }
}
