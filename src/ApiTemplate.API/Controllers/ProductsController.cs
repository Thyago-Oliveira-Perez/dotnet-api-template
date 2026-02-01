namespace ApiTemplate.API.Controllers;

using ApiTemplate.Application.DTOs;
using ApiTemplate.Application.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductService productService, ILogger<ProductsController> logger) : ControllerBase
{
  [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts(CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting all products");
        var products = await productService.GetAllProductsAsync(cancellationToken);
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(Guid id, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting product with id: {ProductId}", id);
        var product = await productService.GetProductByIdAsync(id, cancellationToken);
        
        if (product == null)
        {
            logger.LogWarning("Product with id {ProductId} not found", id);
            return NotFound();
        }

        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto createDto, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating new product: {ProductName}", createDto.Name);
        var product = await productService.CreateProductAsync(createDto, cancellationToken);
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProductDto>> UpdateProduct(Guid id, UpdateProductDto updateDto, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating product with id: {ProductId}", id);
        var product = await productService.UpdateProductAsync(id, updateDto, cancellationToken);
        
        if (product == null)
        {
            logger.LogWarning("Product with id {ProductId} not found for update", id);
            return NotFound();
        }

        return Ok(product);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(Guid id, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting product with id: {ProductId}", id);
        var result = await productService.DeleteProductAsync(id, cancellationToken);
        
        if (!result)
        {
            logger.LogWarning("Product with id {ProductId} not found for deletion", id);
            return NotFound();
        }

        return NoContent();
    }
}
