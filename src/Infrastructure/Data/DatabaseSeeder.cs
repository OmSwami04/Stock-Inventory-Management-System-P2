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
        }

        if (!await _context.Warehouses.AnyAsync())
        {
            var warehouses = new List<Warehouse>
            {
                new Warehouse { WarehouseId = Guid.NewGuid(), WarehouseName = "Global Distribution Hub", Location = "New York, NY", Capacity = 50000 },
                new Warehouse { WarehouseId = Guid.NewGuid(), WarehouseName = "West Coast Fulfillment", Location = "Los Angeles, CA", Capacity = 25000 },
                new Warehouse { WarehouseId = Guid.NewGuid(), WarehouseName = "Midwest Logistics Center", Location = "Chicago, IL", Capacity = 15000 }
            };
            await _context.Warehouses.AddRangeAsync(warehouses);
        }

        if (!await _context.Suppliers.AnyAsync())
        {
            var suppliers = new List<Supplier>
            {
                new Supplier { SupplierId = Guid.NewGuid(), SupplierName = "TechNova Electronics", Email = "sales@technova.com", Phone = "+1 (800) 555-0123", Website = "https://technova.com" },
                new Supplier { SupplierId = Guid.NewGuid(), SupplierName = "LuxeFurnish Co.", Email = "info@luxefurnish.net", Phone = "+1 (800) 555-0456", Website = "https://luxefurnish.net" },
                new Supplier { SupplierId = Guid.NewGuid(), SupplierName = "Global Pantry Supplies", Email = "pantry@globalfood.org", Phone = "+1 (800) 555-0789", Website = "https://globalpantry.org" }
            };
            await _context.Suppliers.AddRangeAsync(suppliers);
        }

        if (!await _context.ProductCategories.AnyAsync())
        {
            var category = new ProductCategory
            {
                CategoryId = Guid.NewGuid(),
                CategoryName = "Electronics",
                Description = "Electronic devices and accessories"
            };
            await _context.ProductCategories.AddAsync(category);

            var furnitureCategory = new ProductCategory
            {
                CategoryId = Guid.NewGuid(),
                CategoryName = "Furniture",
                Description = "Office and home furniture"
            };
            await _context.ProductCategories.AddAsync(furnitureCategory);

            var foodCategory = new ProductCategory
            {
                CategoryId = Guid.NewGuid(),
                CategoryName = "Food & Beverages",
                Description = "Perishable and non-perishable food items"
            };
            await _context.ProductCategories.AddAsync(foodCategory);

            var categories = new List<ProductCategory>
            {
                new ProductCategory { CategoryId = Guid.NewGuid(), CategoryName = "Clothing & Apparel", Description = "Garments, footwear, and accessories" },
                new ProductCategory { CategoryId = Guid.NewGuid(), CategoryName = "Automotive", Description = "Vehicle parts, tools, and maintenance items" },
                new ProductCategory { CategoryId = Guid.NewGuid(), CategoryName = "Health & Beauty", Description = "Personal care, cosmetics, and wellness products" },
                new ProductCategory { CategoryId = Guid.NewGuid(), CategoryName = "Home & Kitchen", Description = "Appliances, cookware, and home decor" },
                new ProductCategory { CategoryId = Guid.NewGuid(), CategoryName = "Sports & Outdoors", Description = "Athletic gear, camping, and fitness equipment" },
                new ProductCategory { CategoryId = Guid.NewGuid(), CategoryName = "Books & Stationery", Description = "Educational materials and office supplies" },
                new ProductCategory { CategoryId = Guid.NewGuid(), CategoryName = "Toys & Games", Description = "Children's toys and entertainment products" }
            };
            await _context.ProductCategories.AddRangeAsync(categories);
        }

        await _context.SaveChangesAsync();
    }
}
