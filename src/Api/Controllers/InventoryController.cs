using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InventoryManagement.Application.Features.Inventory.Queries;
using InventoryManagement.Interfaces.Factories;
using InventoryManagement.Shared.Constants;

namespace InventoryManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InventoryController : ControllerBase
{
    private readonly IMediator _mediator;

    public InventoryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("valuation")]
    [Authorize(Roles = $"{Roles.InventoryManager},{Roles.Admin}")]
    public async Task<IActionResult> GetValuation([FromQuery] ValuationMethod method = ValuationMethod.FIFO)
    {
        var result = await _mediator.Send(new GetInventoryValuationQuery(method));
        return Ok(result);
    }
}
