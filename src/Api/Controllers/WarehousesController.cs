using InventoryManagement.Domain.Entities;
using InventoryManagement.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WarehousesController : ControllerBase
{
    private readonly IGenericRepository<Warehouse> _repository;

    public WarehousesController(IGenericRepository<Warehouse> repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var warehouses = await _repository.GetAllAsync();
        return Ok(warehouses);
    }
}
