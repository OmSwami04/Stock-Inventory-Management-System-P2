namespace InventoryManagement.Domain.Entities;

public class User
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    
    public Guid RoleId { get; set; }
    public Role Role { get; set; } = null!;
}
