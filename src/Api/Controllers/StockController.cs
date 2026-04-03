using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InventoryManagement.Application.Features.Stock.Commands;
using InventoryManagement.Application.Features.Stock.DTOs;
using InventoryManagement.Shared.Constants;
using InventoryManagement.Interfaces.Services;

namespace InventoryManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StockController : ControllerBase
{
    private readonly IStockService _stockService;

    public StockController(IStockService stockService)
    {
        _stockService = stockService;
    }

    [HttpGet]
    [Authorize(Roles = $"{Roles.InventoryClerk},{Roles.InventoryManager},{Roles.Admin}")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _stockService.GetAllStockAsync();
        return Ok(result);
    }

    [HttpGet("{productId}")]
    [Authorize(Roles = $"{Roles.InventoryClerk},{Roles.InventoryManager},{Roles.Admin}")]
    public async Task<IActionResult> GetByProduct(Guid productId)
    {
        var result = await _stockService.GetStockByProductAsync(productId);
        return Ok(result);
    }

    [HttpGet("warehouse/{warehouseId}")]
    [Authorize(Roles = $"{Roles.InventoryClerk},{Roles.InventoryManager},{Roles.Admin}")]
    public async Task<IActionResult> GetByWarehouse(Guid warehouseId)
    {
        var result = await _stockService.GetStockByWarehouseAsync(warehouseId);
        return Ok(result);
    }

    [HttpGet("transactions")]
    [Authorize(Roles = $"{Roles.InventoryClerk},{Roles.InventoryManager},{Roles.Admin}")]
    public async Task<IActionResult> GetTransactions()
    {
        var result = await _stockService.GetStockTransactionsAsync();
        return Ok(result);
    }

    [HttpPost("transfer")]
    [Authorize(Roles = $"{Roles.InventoryClerk},{Roles.InventoryManager},{Roles.Admin}")]
    public async Task<IActionResult> TransferStock([FromBody] StockTransferRequest request)
    {
        try
        {
            await _stockService.TransferStockAsync(
                request.ProductId,
                request.FromWarehouseId,
                request.ToWarehouseId,
                request.Quantity,
                request.ReferenceNumber);
            return Ok(new { message = "Stock transferred successfully" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("transaction")]
    [Authorize(Roles = $"{Roles.InventoryClerk},{Roles.InventoryManager},{Roles.Admin}")]
    public async Task<IActionResult> Transaction([FromBody] CreateStockTransactionCommand command)
    {
        var result = await _stockService.CreateStockTransactionAsync(command.ProductId, command.WarehouseId, command.TransactionType.ToString(), command.Quantity, command.ReferenceNumber, command.TransactionDate);
        return Ok(new { TransactionId = result });
    }
}
