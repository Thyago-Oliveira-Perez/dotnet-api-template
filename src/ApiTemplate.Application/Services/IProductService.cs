namespace ApiTemplate.Application.Services;

using ApiTemplate.Application.DTOs;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllProductsAsync(CancellationToken cancellationToken = default);
    Task<ProductDto?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ProductDto> CreateProductAsync(CreateProductDto createDto, CancellationToken cancellationToken = default);
    Task<ProductDto?> UpdateProductAsync(Guid id, UpdateProductDto updateDto, CancellationToken cancellationToken = default);
    Task<bool> DeleteProductAsync(Guid id, CancellationToken cancellationToken = default);
}
