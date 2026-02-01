namespace ApiTemplate.Tests.Application;

using ApiTemplate.Application.DTOs;
using ApiTemplate.Application.Interfaces;
using ApiTemplate.Application.Services;
using ApiTemplate.Domain.Entities;
using FluentAssertions;
using Moq;

public class ProductServiceTests
{
    private readonly Mock<IRepository<Product>> _mockRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _mockRepository = new Mock<IRepository<Product>>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _service = new ProductService(_mockRepository.Object, _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task GetAllProductsAsync_ShouldReturnAllProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { Id = Guid.NewGuid(), Name = "Product 1", Price = 10.99m, Stock = 5 },
            new() { Id = Guid.NewGuid(), Name = "Product 2", Price = 20.99m, Stock = 10 }
        };
        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        // Act
        var result = await _service.GetAllProductsAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllBeOfType<ProductDto>();
        _mockRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetProductByIdAsync_WhenProductExists_ShouldReturnProduct()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product 
        { 
            Id = productId, 
            Name = "Test Product", 
            Price = 15.99m, 
            Stock = 8 
        };
        _mockRepository.Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Act
        var result = await _service.GetProductByIdAsync(productId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(productId);
        result.Name.Should().Be("Test Product");
        result.Price.Should().Be(15.99m);
        result.Stock.Should().Be(8);
    }

    [Fact]
    public async Task GetProductByIdAsync_WhenProductDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var productId = Guid.NewGuid();
        _mockRepository.Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _service.GetProductByIdAsync(productId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateProductAsync_ShouldCreateAndReturnProduct()
    {
        // Arrange
        var createDto = new CreateProductDto
        {
            Name = "New Product",
            Description = "New Description",
            Price = 29.99m,
            Stock = 15
        };

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product p, CancellationToken ct) => p);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _service.CreateProductAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("New Product");
        result.Description.Should().Be("New Description");
        result.Price.Should().Be(29.99m);
        result.Stock.Should().Be(15);
        result.IsActive.Should().BeTrue();
        result.Id.Should().NotBe(Guid.Empty);

        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateProductAsync_WhenProductExists_ShouldUpdateAndReturnProduct()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var existingProduct = new Product
        {
            Id = productId,
            Name = "Old Name",
            Description = "Old Description",
            Price = 10.00m,
            Stock = 5,
            IsActive = true,
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };

        var updateDto = new UpdateProductDto
        {
            Name = "Updated Name",
            Price = 15.99m,
            Stock = 10
        };

        _mockRepository.Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProduct);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _service.UpdateProductAsync(productId, updateDto);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Updated Name");
        result.Price.Should().Be(15.99m);
        result.Stock.Should().Be(10);
        
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateProductAsync_WhenProductDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var updateDto = new UpdateProductDto { Name = "Updated Name" };

        _mockRepository.Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _service.UpdateProductAsync(productId, updateDto);

        // Assert
        result.Should().BeNull();
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteProductAsync_WhenProductExists_ShouldReturnTrue()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product { Id = productId, Name = "Test Product" };

        _mockRepository.Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _service.DeleteProductAsync(productId);

        // Assert
        result.Should().BeTrue();
        _mockRepository.Verify(r => r.DeleteAsync(product, It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteProductAsync_WhenProductDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var productId = Guid.NewGuid();
        _mockRepository.Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _service.DeleteProductAsync(productId);

        // Assert
        result.Should().BeFalse();
        _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
