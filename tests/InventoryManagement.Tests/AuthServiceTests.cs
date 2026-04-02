using Moq;
using InventoryManagement.Infrastructure.Services;
using InventoryManagement.Interfaces.Repositories;
using InventoryManagement.Interfaces.Auth;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Application.Features.Auth.Commands;
using InventoryManagement.Shared.Exceptions;
using Xunit;

namespace InventoryManagement.Tests;

public class AuthServiceTests
{
    private readonly Mock<IGenericRepository<User>> _userRepoMock;
    private readonly Mock<IGenericRepository<Role>> _roleRepoMock;
    private readonly Mock<IJwtProvider> _jwtProviderMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userRepoMock = new Mock<IGenericRepository<User>>();
        _roleRepoMock = new Mock<IGenericRepository<Role>>();
        _jwtProviderMock = new Mock<IJwtProvider>();
        _passwordHasherMock = new Mock<IPasswordHasher>();

        _authService = new AuthService(
            _userRepoMock.Object,
            _roleRepoMock.Object,
            _jwtProviderMock.Object,
            _passwordHasherMock.Object);
    }

    [Fact]
    public async Task Login_Should_Return_Token_On_Success()
    {
        // Arrange
        var command = new LoginCommand("admin", "Password123!");
        var user = new User { Username = "admin", PasswordHash = "hashed", RoleId = Guid.NewGuid() };
        _userRepoMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<User> { user });
        _passwordHasherMock.Setup(x => x.VerifyPassword("Password123!", "hashed")).Returns(true);
        _jwtProviderMock.Setup(x => x.GenerateToken(user, It.IsAny<string>())).Returns("token");

        // Act
        var result = await _authService.LoginAsync(command);

        // Assert
        Assert.Equal("token", result);
    }

    [Fact]
    public async Task Login_Should_Throw_On_Invalid_Credentials()
    {
        // Arrange
        var command = new LoginCommand("admin", "wrong");
        _userRepoMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<User>());

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _authService.LoginAsync(command));
    }
}
