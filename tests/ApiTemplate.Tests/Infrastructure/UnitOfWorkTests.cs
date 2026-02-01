namespace ApiTemplate.Tests.Infrastructure;

using ApiTemplate.Domain.Entities;
using ApiTemplate.Infrastructure.Data;
using ApiTemplate.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

public class UnitOfWorkTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly UnitOfWork _unitOfWork;

    public UnitOfWorkTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _unitOfWork = new UnitOfWork(_context);
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldPersistChanges()
    {
        // Arrange
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Test Product",
            Price = 10m,
            CreatedAt = DateTime.UtcNow
        };
        await _context.Products.AddAsync(product);

        // Act
        var result = await _unitOfWork.SaveChangesAsync();

        // Assert
        result.Should().Be(1);
        var savedProduct = await _context.Products.FindAsync(product.Id);
        savedProduct.Should().NotBeNull();
    }

    [Fact]
    public async Task SaveChangesAsync_WithMultipleChanges_ShouldReturnCorrectCount()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { Id = Guid.NewGuid(), Name = "Product 1", Price = 10m, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Product 2", Price = 20m, CreatedAt = DateTime.UtcNow }
        };
        await _context.Products.AddRangeAsync(products);

        // Act
        var result = await _unitOfWork.SaveChangesAsync();

        // Assert
        result.Should().Be(2);
    }

    public void Dispose()
    {
        _unitOfWork.Dispose();
        _context.Dispose();
    }
}
