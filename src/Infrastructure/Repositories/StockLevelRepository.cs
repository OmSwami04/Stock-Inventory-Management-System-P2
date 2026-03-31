using InventoryManagement.Domain.Entities;
using InventoryManagement.Infrastructure.Data;
using InventoryManagement.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Repositories;

public class StockLevelRepository : GenericRepository<StockLevel>, IStockLevelRepository
{
    public StockLevelRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<StockLevel?> GetByProductAndWarehouseAsync(Guid productId, Guid warehouseId)
    {
        return await _dbSet.FirstOrDefaultAsync(s => s.ProductId == productId && s.WarehouseId == warehouseId);
    }

    public async Task<IEnumerable<StockLevel>> GetByProductIdAsync(Guid productId)
    {
        return await _dbSet.Where(s => s.ProductId == productId).ToListAsync();
    }

    public async Task<IEnumerable<StockLevel>> GetByWarehouseIdAsync(Guid warehouseId)
    {
        return await _dbSet.Where(s => s.WarehouseId == warehouseId).ToListAsync();
    }
}
