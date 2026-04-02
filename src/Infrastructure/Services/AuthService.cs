using InventoryManagement.Application.Features.Auth.Commands;
using InventoryManagement.Interfaces.Auth;
using InventoryManagement.Interfaces.Repositories;
using InventoryManagement.Interfaces.Services;
using InventoryManagement.Shared.Exceptions;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IGenericRepository<User> _userRepository;
    private readonly IGenericRepository<Role> _roleRepository;
    private readonly IJwtProvider _jwtProvider;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(
        IGenericRepository<User> userRepository,
        IGenericRepository<Role> roleRepository,
        IJwtProvider jwtProvider,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _jwtProvider = jwtProvider;
        _passwordHasher = passwordHasher;
    }

    public async Task<string> LoginAsync(string username, string password)
    {
        var users = await _userRepository.GetAllAsync();
        var user = users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

        if (user == null || !_passwordHasher.VerifyPassword(password, user.PasswordHash))
        {
            throw new BadRequestException("Invalid username or password.");
        }

        var role = await _roleRepository.GetByIdAsync(user.RoleId);
        var roleName = role?.Name ?? "User";

        return _jwtProvider.GenerateToken(user, roleName);
    }

    public async Task RegisterAsync(string username, string password, string roleName)
    {
        var users = await _userRepository.GetAllAsync();
        if (users.Any(u => u.Username == username))
        {
            throw new BadRequestException("Username already exists.");
        }

        var roles = await _roleRepository.GetAllAsync();
        var role = roles.FirstOrDefault(r => r.Name == roleName);
        if (role == null)
        {
            throw new BadRequestException("Invalid role specified.");
        }

        var user = new User
        {
            UserId = Guid.NewGuid(),
            Username = username,
            PasswordHash = _passwordHasher.HashPassword(password),
            RoleId = role.RoleId
        };

        await _userRepository.AddAsync(user);
    }

    public async Task<IEnumerable<string>> GetRolesAsync()
    {
        var roles = await _roleRepository.GetAllAsync();
        return roles.Select(r => r.Name).ToList();
    }

    public async Task CreateRoleAsync(string roleName)
    {
        var roles = await _roleRepository.GetAllAsync();
        if (roles.Any(r => r.Name == roleName))
        {
            throw new BadRequestException("Role already exists.");
        }

        var role = new Role
        {
            RoleId = Guid.NewGuid(),
            Name = roleName
        };

        await _roleRepository.AddAsync(role);
    }
}
