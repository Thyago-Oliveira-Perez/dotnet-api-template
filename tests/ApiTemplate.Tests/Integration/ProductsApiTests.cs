using System.Net;
using System.Net.Http.Json;
using ApiTemplate.Application.DTOs;
using ApiTemplate.Infrastructure.Data;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace ApiTemplate.Tests.Integration;
public class ProductsApiTests(IntegrationTestWebAppFactory factory) : IClassFixture<IntegrationTestWebAppFactory>, IAsyncLifetime
{
    private HttpClient client = null!;

    public async Task InitializeAsync()
    {
        client = factory.CreateClient();
        
        // Clean the database before each test
        using IServiceScope scope = factory.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();
    }

    public Task DisposeAsync()
    {
        client.Dispose();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task GetAllProducts_WhenNoProducts_ShouldReturnEmptyArray()
    {
        // Act
        HttpResponseMessage response = await client.GetAsync("/api/products");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        List<ProductDto>? products = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        products.Should().NotBeNull();
        products.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateProduct_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var createDto = new CreateProductDto
        {
            Name = "Integration Test Product",
            Description = "Test Description",
            Price = 99.99m,
            Stock = 10
        };

        // Act
        HttpResponseMessage response = await client.PostAsJsonAsync("/api/products", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        ProductDto? product = await response.Content.ReadFromJsonAsync<ProductDto>();
        product.Should().NotBeNull();
        product!.Id.Should().NotBe(Guid.Empty);
        product.Name.Should().Be("Integration Test Product");
        product.Description.Should().Be("Test Description");
        product.Price.Should().Be(99.99m);
        product.Stock.Should().Be(10);
        product.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task GetProduct_WhenProductExists_ShouldReturnProduct()
    {
        // Arrange - Create a product first
        var createDto = new CreateProductDto
        {
            Name = "Test Product",
            Description = "Description",
            Price = 49.99m,
            Stock = 5
        };
        HttpResponseMessage createResponse = await client.PostAsJsonAsync("/api/products", createDto);
        ProductDto? createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductDto>();

        // Act
        HttpResponseMessage response = await client.GetAsync($"/api/products/{createdProduct!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        ProductDto? product = await response.Content.ReadFromJsonAsync<ProductDto>();
        product.Should().NotBeNull();
        product!.Id.Should().Be(createdProduct.Id);
        product.Name.Should().Be("Test Product");
    }

    [Fact]
    public async Task GetProduct_WhenProductDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        HttpResponseMessage response = await client.GetAsync($"/api/products/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateProduct_WhenProductExists_ShouldReturnOk()
    {
        // Arrange - Create a product first
        var createDto = new CreateProductDto
        {
            Name = "Original Product",
            Price = 10.00m,
            Stock = 5
        };
        HttpResponseMessage createResponse = await client.PostAsJsonAsync("/api/products", createDto);
        ProductDto? createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductDto>();

        var updateDto = new UpdateProductDto
        {
            Name = "Updated Product",
            Description = "Updated Description",
            Price = 20.00m,
            Stock = 15
        };

        // Act
        HttpResponseMessage response = await client.PutAsJsonAsync($"/api/products/{createdProduct!.Id}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        ProductDto? updatedProduct = await response.Content.ReadFromJsonAsync<ProductDto>();
        updatedProduct.Should().NotBeNull();
        updatedProduct!.Name.Should().Be("Updated Product");
        updatedProduct.Description.Should().Be("Updated Description");
        updatedProduct.Price.Should().Be(20.00m);
        updatedProduct.Stock.Should().Be(15);
    }

    [Fact]
    public async Task UpdateProduct_WhenProductDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var updateDto = new UpdateProductDto
        {
            Name = "Updated Product",
            Price = 20.00m
        };

        // Act
        HttpResponseMessage response = await client.PutAsJsonAsync($"/api/products/{nonExistentId}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteProduct_WhenProductExists_ShouldReturnNoContent()
    {
        // Arrange - Create a product first
        var createDto = new CreateProductDto
        {
            Name = "Product to Delete",
            Price = 10.00m,
            Stock = 5
        };
        HttpResponseMessage createResponse = await client.PostAsJsonAsync("/api/products", createDto);
        ProductDto? createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductDto>();

        // Act
        HttpResponseMessage response = await client.DeleteAsync($"/api/products/{createdProduct!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify product is deleted
        HttpResponseMessage getResponse = await client.GetAsync($"/api/products/{createdProduct.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteProduct_WhenProductDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        HttpResponseMessage response = await client.DeleteAsync($"/api/products/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CompleteProductLifecycle_ShouldWorkEndToEnd()
    {
        // 1. Create a product
        var createDto = new CreateProductDto
        {
            Name = "Lifecycle Product",
            Description = "Testing full lifecycle",
            Price = 29.99m,
            Stock = 100
        };
        HttpResponseMessage createResponse = await client.PostAsJsonAsync("/api/products", createDto);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        ProductDto? product = await createResponse.Content.ReadFromJsonAsync<ProductDto>();
        product.Should().NotBeNull();

        // 2. Get the product
        HttpResponseMessage getResponse = await client.GetAsync($"/api/products/{product!.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        ProductDto? retrievedProduct = await getResponse.Content.ReadFromJsonAsync<ProductDto>();
        retrievedProduct!.Name.Should().Be("Lifecycle Product");

        // 3. Update the product
        var updateDto = new UpdateProductDto
        {
            Name = "Updated Lifecycle Product",
            Price = 39.99m,
            Stock = 50
        };
        HttpResponseMessage updateResponse = await client.PutAsJsonAsync($"/api/products/{product.Id}", updateDto);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        ProductDto? updatedProduct = await updateResponse.Content.ReadFromJsonAsync<ProductDto>();
        updatedProduct!.Name.Should().Be("Updated Lifecycle Product");
        updatedProduct.Price.Should().Be(39.99m);

        // 4. Verify the update
        HttpResponseMessage getUpdatedResponse = await client.GetAsync($"/api/products/{product.Id}");
        ProductDto? finalProduct = await getUpdatedResponse.Content.ReadFromJsonAsync<ProductDto>();
        finalProduct!.Name.Should().Be("Updated Lifecycle Product");
        finalProduct.Stock.Should().Be(50);

        // 5. Delete the product
        HttpResponseMessage deleteResponse = await client.DeleteAsync($"/api/products/{product.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // 6. Verify deletion
        HttpResponseMessage getDeletedResponse = await client.GetAsync($"/api/products/{product.Id}");
        getDeletedResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAllProducts_AfterCreatingMultiple_ShouldReturnAllProducts()
    {
        // Arrange - Create multiple products
        CreateProductDto[] products =
        [
          new CreateProductDto { Name = "Product 1", Price = 10m, Stock = 5 },
          new CreateProductDto { Name = "Product 2", Price = 20m, Stock = 10 },
          new CreateProductDto { Name = "Product 3", Price = 30m, Stock = 15 }
        ];

        foreach (CreateProductDto productDto in products)
        {
            HttpResponseMessage createResponse = await client.PostAsJsonAsync("/api/products", productDto);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        // Act
        HttpResponseMessage response = await client.GetAsync("/api/products");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        List<ProductDto>? allProducts = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        allProducts.Should().NotBeNull();
        allProducts.Should().HaveCountGreaterThanOrEqualTo(3);
        allProducts.Should().Contain(p => p.Name == "Product 1");
        allProducts.Should().Contain(p => p.Name == "Product 2");
        allProducts.Should().Contain(p => p.Name == "Product 3");
    }
}
