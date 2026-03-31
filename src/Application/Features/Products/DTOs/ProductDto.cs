namespace InventoryManagement.Application.Features.Products.DTOs;

public class ProductDto
{
    public Guid ProductId { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string UnitOfMeasure { get; set; } = string.Empty;
    public decimal Cost { get; set; }
    public decimal ListPrice { get; set; }
    public bool IsActive { get; set; }
}
