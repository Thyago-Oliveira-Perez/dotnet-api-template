namespace ApiTemplate.Tests.Infrastructure;

using ApiTemplate.Domain.Entities;
using ApiTemplate.Infrastructure.Data;
using ApiTemplate.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

public class RepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Repository<Product> _repository;

    public RepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new Repository<Product>(_context);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnProduct_WhenExists()
    {
        // Arrange
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Test Product",
            Price = 10.99m,
            Stock = 5,
            CreatedAt = DateTime.UtcNow
        };
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(product.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(product.Id);
        result.Name.Should().Be("Test Product");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.GetByIdAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { Id = Guid.NewGuid(), Name = "Product 1", Price = 10m, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Product 2", Price = 20m, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Product 3", Price = 30m, CreatedAt = DateTime.UtcNow }
        };
        await _context.Products.AddRangeAsync(products);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(p => p.Name == "Product 1");
        result.Should().Contain(p => p.Name == "Product 2");
        result.Should().Contain(p => p.Name == "Product 3");
    }

    [Fact]
    public async Task FindAsync_ShouldReturnMatchingProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { Id = Guid.NewGuid(), Name = "Active Product 1", Price = 10m, IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Active Product 2", Price = 20m, IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Inactive Product", Price = 30m, IsActive = false, CreatedAt = DateTime.UtcNow }
        };
        await _context.Products.AddRangeAsync(products);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.FindAsync(p => p.IsActive);

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(p => p.IsActive.Should().BeTrue());
    }

    [Fact]
    public async Task AddAsync_ShouldAddProduct()
    {
        // Arrange
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "New Product",
            Price = 15.99m,
            Stock = 10,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var result = await _repository.AddAsync(product);
        await _context.SaveChangesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(product.Id);
        
        var savedProduct = await _context.Products.FindAsync(product.Id);
        savedProduct.Should().NotBeNull();
        savedProduct!.Name.Should().Be("New Product");
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateProduct()
    {
        // Arrange
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Original Name",
            Price = 10m,
            CreatedAt = DateTime.UtcNow
        };
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        // Act
        product.Name = "Updated Name";
        product.Price = 20m;
        await _repository.UpdateAsync(product);
        await _context.SaveChangesAsync();

        // Assert
        var updatedProduct = await _context.Products.FindAsync(product.Id);
        updatedProduct.Should().NotBeNull();
        updatedProduct!.Name.Should().Be("Updated Name");
        updatedProduct.Price.Should().Be(20m);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveProduct()
    {
        // Arrange
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Product to Delete",
            Price = 10m,
            CreatedAt = DateTime.UtcNow
        };
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(product);
        await _context.SaveChangesAsync();

        // Assert
        var deletedProduct = await _context.Products.FindAsync(product.Id);
        deletedProduct.Should().BeNull();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
