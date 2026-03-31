using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InventoryManagement.Application.Features.Products.Commands;
using InventoryManagement.Application.Features.Products.Queries;
using InventoryManagement.Shared.Constants;

namespace InventoryManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Roles = $"{Roles.InventoryClerk},{Roles.InventoryManager},{Roles.Admin}")]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetAllProductsQuery(pageNumber, pageSize);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = $"{Roles.InventoryClerk},{Roles.InventoryManager},{Roles.Admin}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetProductByIdQuery(id));
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = $"{Roles.InventoryManager},{Roles.Admin}")]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result }, null);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = $"{Roles.InventoryManager},{Roles.Admin}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductCommand command)
    {
        if (id != command.ProductId) return BadRequest();
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteProductCommand(id));
        return NoContent();
    }
}
