using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InventoryManagement.Application.Features.Products.Commands;
using InventoryManagement.Application.Features.Products.Queries;
using InventoryManagement.Shared.Constants;
using InventoryManagement.Interfaces.Services;

namespace InventoryManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    [Authorize(Roles = $"{Roles.InventoryClerk},{Roles.InventoryManager},{Roles.Admin}")]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _productService.GetAllProductsAsync(pageNumber, pageSize);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = $"{Roles.InventoryClerk},{Roles.InventoryManager},{Roles.Admin}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _productService.GetProductByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = $"{Roles.InventoryManager},{Roles.Admin}")]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
    {
        var result = await _productService.CreateProductAsync(command.SKU, command.ProductName, command.Description, command.CategoryId, command.UnitOfMeasure, command.Cost, command.ListPrice, command.ReorderLevel, command.SafetyStock);
        return CreatedAtAction(nameof(GetById), new { id = result }, null);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = $"{Roles.InventoryManager},{Roles.Admin}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductCommand command)
    {
        if (id != command.ProductId) return BadRequest();
        await _productService.UpdateProductAsync(command.ProductId, command.ProductName, command.Description, command.CategoryId, command.UnitOfMeasure, command.Cost, command.ListPrice, command.ReorderLevel, command.SafetyStock, command.IsActive);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _productService.DeleteProductAsync(id);
        return NoContent();
    }
}
