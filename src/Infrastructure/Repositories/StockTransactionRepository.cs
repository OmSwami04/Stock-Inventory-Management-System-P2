using InventoryManagement.Domain.Entities;
using InventoryManagement.Interfaces.Repositories;
using InventoryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Repositories;

public class StockTransactionRepository : GenericRepository<StockTransaction>, IStockTransactionRepository
{
    public StockTransactionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<StockTransaction>> GetByProductIdAsync(Guid productId)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(st => st.Product)
            .Include(st => st.Warehouse)
            .Where(st => st.ProductId == productId)
            .OrderByDescending(st => st.TransactionDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<StockTransaction>> GetAllWithDetailsAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .Include(st => st.Product)
            .Include(st => st.Warehouse)
            .OrderByDescending(st => st.TransactionDate)
            .ToListAsync();
    }
}
