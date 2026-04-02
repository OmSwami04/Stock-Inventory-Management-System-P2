using Microsoft.AspNetCore.Mvc;
using InventoryManagement.Application.Features.Auth.Commands;
using InventoryManagement.Interfaces.Services;

namespace InventoryManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var token = await _authService.LoginAsync(command.Username, command.Password);
        return Ok(new { Token = token });
    }
}
