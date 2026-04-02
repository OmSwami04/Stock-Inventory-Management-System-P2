using MediatR;
using InventoryManagement.Application.Features.Auth.Commands;
using InventoryManagement.Interfaces.Repositories;
using InventoryManagement.Interfaces.Auth;
using InventoryManagement.Shared.Exceptions;

namespace InventoryManagement.Application.Features.Auth.Handlers;

public class LoginCommandHandler : IRequestHandler<LoginCommand, string>
{
    private readonly IGenericRepository<Domain.Entities.User> _userRepository;
    private readonly IGenericRepository<Domain.Entities.Role> _roleRepository;
    private readonly IJwtProvider _jwtProvider;
    private readonly IPasswordHasher _passwordHasher;

    public LoginCommandHandler(
        IGenericRepository<Domain.Entities.User> userRepository,
        IGenericRepository<Domain.Entities.Role> roleRepository,
        IJwtProvider jwtProvider,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _jwtProvider = jwtProvider;
        _passwordHasher = passwordHasher;
    }

    public async Task<string> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllAsync();
        var user = users.FirstOrDefault(u => u.Username.Equals(request.Username, StringComparison.OrdinalIgnoreCase));

        if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new BadRequestException("Invalid username or password.");
        }

        var role = await _roleRepository.GetByIdAsync(user.RoleId);
        var roleName = role?.Name ?? "User";

        return _jwtProvider.GenerateToken(user, roleName);
    }
}
