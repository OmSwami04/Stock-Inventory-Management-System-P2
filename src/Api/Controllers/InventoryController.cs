using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InventoryManagement.Application.Features.Inventory.Queries;
using InventoryManagement.Interfaces.Factories;
using InventoryManagement.Shared.Constants;
using InventoryManagement.Interfaces.Services;

namespace InventoryManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpGet("valuation")]
    [Authorize(Roles = $"{Roles.InventoryManager},{Roles.Admin}")]
    public async Task<IActionResult> GetValuation([FromQuery] ValuationMethod method = ValuationMethod.FIFO)
    {
        var result = await _inventoryService.GetInventoryValuationAsync(method.ToString());
        return Ok(result);
    }

    [HttpGet("alerts/low-stock")]
    [Authorize(Roles = $"{Roles.InventoryManager},{Roles.Admin}")]
    public async Task<IActionResult> GetLowStockAlerts()
    {
        var result = await _inventoryService.GetLowStockAlertsCountAsync();
        return Ok(new { Count = result });
    }

    [HttpGet("analytics/total-value")]
    [Authorize(Roles = $"{Roles.InventoryManager},{Roles.Admin}")]
    public async Task<IActionResult> GetTotalValue()
    {
        var result = await _inventoryService.GetTotalInventoryValueAsync();
        return Ok(new { TotalValue = result });
    }
}
