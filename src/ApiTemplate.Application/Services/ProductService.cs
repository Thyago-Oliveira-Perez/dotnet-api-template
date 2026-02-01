namespace ApiTemplate.Application.Services;

using ApiTemplate.Application.DTOs;
using ApiTemplate.Application.Interfaces;
using ApiTemplate.Domain.Entities;

public class ProductService(IRepository<Product> productRepository, IUnitOfWork unitOfWork) : IProductService
{
  public async Task<IEnumerable<ProductDto>> GetAllProductsAsync(CancellationToken cancellationToken = default)
    {
        var products = await productRepository.GetAllAsync(cancellationToken);
        return products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            Stock = p.Stock,
            IsActive = p.IsActive
        });
    }

    public async Task<ProductDto?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetByIdAsync(id, cancellationToken);
        if (product == null) return null;

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            IsActive = product.IsActive
        };
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto createDto, CancellationToken cancellationToken = default)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = createDto.Name,
            Description = createDto.Description,
            Price = createDto.Price,
            Stock = createDto.Stock,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await productRepository.AddAsync(product, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            IsActive = product.IsActive
        };
    }

    public async Task<ProductDto?> UpdateProductAsync(Guid id, UpdateProductDto updateDto, CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetByIdAsync(id, cancellationToken);
        if (product == null) return null;

        if (updateDto.Name != null) product.Name = updateDto.Name;
        if (updateDto.Description != null) product.Description = updateDto.Description;
        if (updateDto.Price.HasValue) product.Price = updateDto.Price.Value;
        if (updateDto.Stock.HasValue) product.Stock = updateDto.Stock.Value;
        if (updateDto.IsActive.HasValue) product.IsActive = updateDto.IsActive.Value;
        product.UpdatedAt = DateTime.UtcNow;

        await productRepository.UpdateAsync(product, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            IsActive = product.IsActive
        };
    }

    public async Task<bool> DeleteProductAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetByIdAsync(id, cancellationToken);
        if (product == null) return false;

        await productRepository.DeleteAsync(product, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
