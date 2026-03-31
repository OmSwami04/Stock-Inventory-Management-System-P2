using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Interfaces.Repositories;

public interface IStockLevelRepository : IGenericRepository<StockLevel>
{
    Task<StockLevel?> GetByProductAndWarehouseAsync(Guid productId, Guid warehouseId);
    Task<IEnumerable<StockLevel>> GetByProductIdAsync(Guid productId);
    Task<IEnumerable<StockLevel>> GetByWarehouseIdAsync(Guid warehouseId);
}
