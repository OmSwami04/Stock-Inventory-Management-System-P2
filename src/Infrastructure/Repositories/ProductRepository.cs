using InventoryManagement.Domain.Entities;
using InventoryManagement.Interfaces.Repositories;
using InventoryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> SkuExistsAsync(string sku)
    {
        return await _dbSet.AnyAsync(p => p.SKU == sku);
    }

    public async Task<IEnumerable<Product>> GetPagedAsync(int pageNumber, int pageSize)
    {
        return await _dbSet
            .AsNoTracking()
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetTotalCountAsync()
    {
        return await _dbSet.CountAsync();
    }
}
