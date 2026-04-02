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

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        await _authService.RegisterAsync(command.Username, command.Password, command.RoleName);
        return Ok(new { Message = "User registered successfully." });
    }

    [HttpGet("roles")]
    public async Task<IActionResult> GetRoles()
    {
        var roles = await _authService.GetRolesAsync();
        return Ok(roles);
    }

    [HttpPost("roles")]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleCommand command)
    {
        await _authService.CreateRoleAsync(command.RoleName);
        return Ok(new { Message = "Role created successfully." });
    }
}
