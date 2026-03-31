using InventoryManagement.Domain.Entities;
using InventoryManagement.Interfaces.Auth;
using Microsoft.EntityFrameworkCore;
using InventoryManagement.Shared.Constants;

namespace InventoryManagement.Infrastructure.Data;

public class DatabaseSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public DatabaseSeeder(ApplicationDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task SeedAsync()
    {
        await _context.Database.MigrateAsync();

        if (!await _context.Roles.AnyAsync())
        {
            var roles = new List<Role>
            {
                new Role { RoleId = Guid.NewGuid(), Name = Roles.Admin },
                new Role { RoleId = Guid.NewGuid(), Name = Roles.InventoryManager },
                new Role { RoleId = Guid.NewGuid(), Name = Roles.InventoryClerk }
            };
            await _context.Roles.AddRangeAsync(roles);
            await _context.SaveChangesAsync();
        }

        if (!await _context.Users.AnyAsync())
        {
            var adminRole = await _context.Roles.FirstAsync(r => r.Name == Roles.Admin);
            var adminUser = new User
            {
                UserId = Guid.NewGuid(),
                Username = "admin",
                PasswordHash = _passwordHasher.HashPassword("Password123!"),
                RoleId = adminRole.RoleId
            };
            await _context.Users.AddAsync(adminUser);

            // Add a sample Warehouse and Category for testing
            var warehouse = new Warehouse
            {
                WarehouseId = Guid.NewGuid(),
                WarehouseName = "Main Warehouse",
                Location = "New York",
                Capacity = 10000
            };
            await _context.Warehouses.AddAsync(warehouse);

            var category = new ProductCategory
            {
                CategoryId = Guid.NewGuid(),
                CategoryName = "Electronics",
                Description = "Electronic devices and accessories"
            };
            await _context.ProductCategories.AddAsync(category);

            await _context.SaveChangesAsync();
        }
    }
}
