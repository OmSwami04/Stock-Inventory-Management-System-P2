namespace InventoryManagement.Domain.Entities;

public class ProductCategory
{
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid? ParentCategoryId { get; set; }

    // Navigation properties
    public ProductCategory? ParentCategory { get; set; }
    public ICollection<ProductCategory> SubCategories { get; set; } = new List<ProductCategory>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
