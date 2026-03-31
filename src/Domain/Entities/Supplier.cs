namespace InventoryManagement.Domain.Entities;

public class Supplier
{
    public Guid SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;

    // Navigation properties
    public ICollection<ProductSupplier> ProductSuppliers { get; set; } = new List<ProductSupplier>();
}
