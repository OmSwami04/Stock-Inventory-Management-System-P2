namespace InventoryManagement.Domain.Entities;

public class Warehouse
{
    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int Capacity { get; set; }

    // Navigation properties
    public ICollection<StockLevel> StockLevels { get; set; } = new List<StockLevel>();
}
