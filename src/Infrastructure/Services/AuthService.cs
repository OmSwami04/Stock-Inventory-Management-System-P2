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
        var user = users.FirstOrDefault(u => u.Username == username);

        if (user == null || !_passwordHasher.VerifyPassword(password, user.PasswordHash))
        {
            throw new BadRequestException("Invalid username or password.");
        }

        var role = await _roleRepository.GetByIdAsync(user.RoleId);
        var roleName = role?.Name ?? "User";

        return _jwtProvider.GenerateToken(user, roleName);
    }
}
