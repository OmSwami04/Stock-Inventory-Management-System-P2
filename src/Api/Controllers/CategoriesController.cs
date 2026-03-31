using InventoryManagement.Domain.Entities;
using InventoryManagement.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly IGenericRepository<ProductCategory> _repository;

    public CategoriesController(IGenericRepository<ProductCategory> repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _repository.GetAllAsync();
        return Ok(categories);
    }
}
