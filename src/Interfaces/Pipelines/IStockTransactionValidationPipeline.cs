using InventoryManagement.Domain.Entities;
using InventoryManagement.Domain.Enums;

namespace InventoryManagement.Interfaces.Pipelines;

public class TransactionRequestContext
{
    public Guid ProductId { get; set; }
    public Guid WarehouseId { get; set; }
    public int Quantity { get; set; }
    public TransactionType TransactionType { get; set; }
    public Product? Product { get; set; }
    public Warehouse? Warehouse { get; set; }
    public StockLevel? Stock { get; set; }
}

public interface IStockTransactionValidationPipeline
{
    Task ValidateAsync(TransactionRequestContext context);
}
