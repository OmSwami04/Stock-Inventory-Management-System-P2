namespace InventoryManagement.Domain.Entities;

public class ProductSupplier
{
    public Guid SupplierProductId { get; set; }
    public Guid ProductId { get; set; }
    public Guid SupplierId { get; set; }
    public string SupplierSKU { get; set; } = string.Empty;
    public int LeadTime { get; set; }

    // Navigation properties
    public Product? Product { get; set; }
    public Supplier? Supplier { get; set; }
}
