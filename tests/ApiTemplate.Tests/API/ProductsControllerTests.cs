namespace ApiTemplate.Tests.API;

using ApiTemplate.API.Controllers;
using ApiTemplate.Application.DTOs;
using ApiTemplate.Application.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

public class ProductsControllerTests
{
    private readonly Mock<IProductService> _mockService;
    private readonly Mock<ILogger<ProductsController>> _mockLogger;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        _mockService = new Mock<IProductService>();
        _mockLogger = new Mock<ILogger<ProductsController>>();
        _controller = new ProductsController(_mockService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAllProducts_ShouldReturnOkWithProducts()
    {
        // Arrange
        var products = new List<ProductDto>
        {
            new() { Id = Guid.NewGuid(), Name = "Product 1", Price = 10m, Stock = 5 },
            new() { Id = Guid.NewGuid(), Name = "Product 2", Price = 20m, Stock = 10 }
        };
        _mockService.Setup(s => s.GetAllProductsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        // Act
        var result = await _controller.GetAllProducts(CancellationToken.None);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProducts = okResult.Value.Should().BeAssignableTo<IEnumerable<ProductDto>>().Subject;
        returnedProducts.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetProduct_WhenProductExists_ShouldReturnOk()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new ProductDto 
        { 
            Id = productId, 
            Name = "Test Product", 
            Price = 15m, 
            Stock = 8 
        };
        _mockService.Setup(s => s.GetProductByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Act
        var result = await _controller.GetProduct(productId, CancellationToken.None);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProduct = okResult.Value.Should().BeOfType<ProductDto>().Subject;
        returnedProduct.Id.Should().Be(productId);
        returnedProduct.Name.Should().Be("Test Product");
    }

    [Fact]
    public async Task GetProduct_WhenProductDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var productId = Guid.NewGuid();
        _mockService.Setup(s => s.GetProductByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductDto?)null);

        // Act
        var result = await _controller.GetProduct(productId, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task CreateProduct_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var createDto = new CreateProductDto
        {
            Name = "New Product",
            Description = "Description",
            Price = 25m,
            Stock = 15
        };
        var createdProduct = new ProductDto
        {
            Id = Guid.NewGuid(),
            Name = "New Product",
            Description = "Description",
            Price = 25m,
            Stock = 15,
            IsActive = true
        };
        _mockService.Setup(s => s.CreateProductAsync(createDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdProduct);

        // Act
        var result = await _controller.CreateProduct(createDto, CancellationToken.None);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(ProductsController.GetProduct));
        var returnedProduct = createdResult.Value.Should().BeOfType<ProductDto>().Subject;
        returnedProduct.Name.Should().Be("New Product");
    }

    [Fact]
    public async Task UpdateProduct_WhenProductExists_ShouldReturnOk()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var updateDto = new UpdateProductDto
        {
            Name = "Updated Product",
            Price = 30m
        };
        var updatedProduct = new ProductDto
        {
            Id = productId,
            Name = "Updated Product",
            Price = 30m,
            Stock = 5,
            IsActive = true
        };
        _mockService.Setup(s => s.UpdateProductAsync(productId, updateDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(updatedProduct);

        // Act
        var result = await _controller.UpdateProduct(productId, updateDto, CancellationToken.None);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProduct = okResult.Value.Should().BeOfType<ProductDto>().Subject;
        returnedProduct.Name.Should().Be("Updated Product");
    }

    [Fact]
    public async Task UpdateProduct_WhenProductDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var updateDto = new UpdateProductDto { Name = "Updated Product" };
        _mockService.Setup(s => s.UpdateProductAsync(productId, updateDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductDto?)null);

        // Act
        var result = await _controller.UpdateProduct(productId, updateDto, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeleteProduct_WhenProductExists_ShouldReturnNoContent()
    {
        // Arrange
        var productId = Guid.NewGuid();
        _mockService.Setup(s => s.DeleteProductAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteProduct(productId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteProduct_WhenProductDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var productId = Guid.NewGuid();
        _mockService.Setup(s => s.DeleteProductAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteProduct(productId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }
}
