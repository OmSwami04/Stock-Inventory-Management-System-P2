using InventoryManagement.Domain.Entities;
using InventoryManagement.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SuppliersController : ControllerBase
{
    private readonly IGenericRepository<Supplier> _repository;

    public SuppliersController(IGenericRepository<Supplier> repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var suppliers = await _repository.GetAllAsync();
        return Ok(suppliers);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var supplier = await _repository.GetByIdAsync(id);
        if (supplier == null) return NotFound();
        return Ok(supplier);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Supplier supplier)
    {
        supplier.SupplierId = Guid.NewGuid();
        await _repository.AddAsync(supplier);
        await _repository.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = supplier.SupplierId }, supplier);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Supplier supplier)
    {
        if (id != supplier.SupplierId) return BadRequest();
        _repository.Update(supplier);
        await _repository.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var supplier = await _repository.GetByIdAsync(id);
        if (supplier == null) return NotFound();
        _repository.Delete(supplier);
        await _repository.SaveChangesAsync();
        return NoContent();
    }
}
