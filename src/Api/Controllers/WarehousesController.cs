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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var warehouse = await _repository.GetByIdAsync(id);
        if (warehouse == null) return NotFound();
        return Ok(warehouse);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Warehouse warehouse)
    {
        warehouse.WarehouseId = Guid.NewGuid();
        await _repository.AddAsync(warehouse);
        await _repository.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = warehouse.WarehouseId }, warehouse);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Warehouse warehouse)
    {
        if (id != warehouse.WarehouseId) return BadRequest();
        _repository.Update(warehouse);
        await _repository.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var warehouse = await _repository.GetByIdAsync(id);
        if (warehouse == null) return NotFound();
        _repository.Delete(warehouse);
        await _repository.SaveChangesAsync();
        return NoContent();
    }
}
