using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Interfaces.Auth;

public interface IJwtProvider
{
    string GenerateToken(User user, string roleName);
}
