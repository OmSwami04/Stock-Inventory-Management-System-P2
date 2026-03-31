using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InventoryManagement.Application.Features.Stock.Commands;
using InventoryManagement.Application.Features.Stock.Queries;
using InventoryManagement.Shared.Constants;

namespace InventoryManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StockController : ControllerBase
{
    private readonly IMediator _mediator;

    public StockController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Roles = $"{Roles.InventoryClerk},{Roles.InventoryManager},{Roles.Admin}")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllStockQuery());
        return Ok(result);
    }

    [HttpGet("{productId}")]
    [Authorize(Roles = $"{Roles.InventoryClerk},{Roles.InventoryManager},{Roles.Admin}")]
    public async Task<IActionResult> GetByProduct(Guid productId)
    {
        var result = await _mediator.Send(new GetStockByProductQuery(productId));
        return Ok(result);
    }

    [HttpGet("warehouse/{warehouseId}")]
    [Authorize(Roles = $"{Roles.InventoryClerk},{Roles.InventoryManager},{Roles.Admin}")]
    public async Task<IActionResult> GetByWarehouse(Guid warehouseId)
    {
        var result = await _mediator.Send(new GetStockByWarehouseQuery(warehouseId));
        return Ok(result);
    }

    [HttpPost("transaction")]
    [Authorize(Roles = $"{Roles.InventoryClerk},{Roles.InventoryManager},{Roles.Admin}")]
    public async Task<IActionResult> Transaction([FromBody] CreateStockTransactionCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new { TransactionId = result });
    }
}
