namespace InventoryManagement.Domain.Entities;

public class StockLevel
{
    public Guid StockLevelId { get; set; }
    public Guid ProductId { get; set; }
    public Guid WarehouseId { get; set; }
    public int QuantityOnHand { get; set; }
    public int ReorderLevel { get; set; }
    public int SafetyStock { get; set; }

    // Navigation properties
    public Product? Product { get; set; }
    public Warehouse? Warehouse { get; set; }
}
