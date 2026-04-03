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
        var username = "admin";
        var password = "Password123!";
        var user = new User { Username = "admin", PasswordHash = "hashed", RoleId = Guid.NewGuid() };
        _userRepoMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<User> { user });
        _passwordHasherMock.Setup(x => x.VerifyPassword(password, "hashed")).Returns(true);
        _jwtProviderMock.Setup(x => x.GenerateToken(user, It.IsAny<string>())).Returns("token");

        // Act
        var result = await _authService.LoginAsync(username, password);

        // Assert
        Assert.Equal("token", result);
    }

    [Fact]
    public async Task Login_Should_Throw_On_Invalid_Credentials()
    {
        // Arrange
        var username = "admin";
        var password = "wrong";
        _userRepoMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<User>());

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _authService.LoginAsync(username, password));
    }

    [Fact]
    public async Task Register_Should_Add_User()
    {
        // Arrange
        var username = "newuser";
        var password = "password";
        var roleName = "Admin";
        _userRepoMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<User>());
        _roleRepoMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Role> { new Role { Name = roleName, RoleId = Guid.NewGuid() } });

        // Act
        await _authService.RegisterAsync(username, password, roleName);

        // Assert
        _userRepoMock.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task Register_Should_Throw_If_User_Exists()
    {
        // Arrange
        var username = "exists";
        _userRepoMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<User> { new User { Username = username } });

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _authService.RegisterAsync(username, "pass", "Admin"));
    }

    [Fact]
    public async Task CreateRole_Should_Add_Role()
    {
        // Arrange
        var roleName = "Manager";
        _roleRepoMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Role>());

        // Act
        await _authService.CreateRoleAsync(roleName);

        // Assert
        _roleRepoMock.Verify(x => x.AddAsync(It.IsAny<Role>()), Times.Once);
    }

    [Fact]
    public async Task GetRoles_Should_Return_List()
    {
        // Arrange
        var roles = new List<Role> { new Role { Name = "Admin" }, new Role { Name = "User" } };
        _roleRepoMock.Setup(x => x.GetAllAsync()).ReturnsAsync(roles);

        // Act
        var result = await _authService.GetRolesAsync();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains("Admin", result);
    }
}
