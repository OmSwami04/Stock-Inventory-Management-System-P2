using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Interfaces.Repositories;

public interface IStockTransactionRepository : IGenericRepository<StockTransaction>
{
    Task<IEnumerable<StockTransaction>> GetByProductIdAsync(Guid productId);
    Task<IEnumerable<StockTransaction>> GetAllWithDetailsAsync();
}
