namespace ApiTemplate.Tests.Domain;

using ApiTemplate.Domain.Entities;
using FluentAssertions;

public class ProductTests
{
    [Fact]
    public void Product_ShouldInitialize_WithDefaultValues()
    {
        // Act
        var product = new Product();

        // Assert
        product.Id.Should().Be(Guid.Empty);
        product.Name.Should().BeEmpty();
        product.Description.Should().BeEmpty();
        product.Price.Should().Be(0);
        product.Stock.Should().Be(0);
        product.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Product_ShouldAllowSettingProperties()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;

        // Act
        var product = new Product
        {
            Id = productId,
            Name = "Test Product",
            Description = "Test Description",
            Price = 99.99m,
            Stock = 10,
            IsActive = true,
            CreatedAt = createdAt,
            UpdatedAt = createdAt
        };

        // Assert
        product.Id.Should().Be(productId);
        product.Name.Should().Be("Test Product");
        product.Description.Should().Be("Test Description");
        product.Price.Should().Be(99.99m);
        product.Stock.Should().Be(10);
        product.IsActive.Should().BeTrue();
        product.CreatedAt.Should().Be(createdAt);
        product.UpdatedAt.Should().Be(createdAt);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    [InlineData(1000)]
    public void Product_ShouldAcceptAnyStockValue(int stock)
    {
        // Arrange & Act
        var product = new Product { Stock = stock };

        // Assert
        product.Stock.Should().Be(stock);
    }

    [Theory]
    [InlineData(0.01)]
    [InlineData(999999.99)]
    public void Product_ShouldAcceptValidPriceRange(decimal price)
    {
        // Arrange & Act
        var product = new Product { Price = price };

        // Assert
        product.Price.Should().Be(price);
    }
}
