namespace InventoryManagement.Application.Features.Stock.DTOs;

public class StockLevelDto
{
    public Guid StockLevelId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public int QuantityOnHand { get; set; }
    public int ReorderLevel { get; set; }
    public int SafetyStock { get; set; }
}
