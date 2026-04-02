namespace InventoryManagement.Interfaces.Services;

public interface IAuthService
{
    Task<string> LoginAsync(string username, string password);
}

public interface IProductService
{
    Task<Guid> CreateProductAsync(string sku, string productName, string description, Guid categoryId, string unitOfMeasure, decimal cost, decimal listPrice, int reorderLevel, int safetyStock);
    Task UpdateProductAsync(Guid productId, string productName, string description, Guid categoryId, string unitOfMeasure, decimal cost, decimal listPrice, int reorderLevel, int safetyStock, bool isActive);
    Task DeleteProductAsync(Guid id);
    Task<object> GetProductByIdAsync(Guid id);
    Task<object> GetAllProductsAsync(int pageNumber, int pageSize);
}

public interface IStockService
{
    Task<Guid> CreateStockTransactionAsync(Guid productId, Guid warehouseId, string transactionType, int quantity, string referenceNumber, DateTime transactionDate);
    Task<IEnumerable<object>> GetAllStockAsync();
    Task<IEnumerable<object>> GetStockByProductAsync(Guid productId);
    Task<IEnumerable<object>> GetStockByWarehouseAsync(Guid warehouseId);
}

public interface IInventoryService
{
    Task<object> GetInventoryValuationAsync(string method);
    Task<int> GetLowStockAlertsCountAsync();
    Task<decimal> GetTotalInventoryValueAsync();
}
