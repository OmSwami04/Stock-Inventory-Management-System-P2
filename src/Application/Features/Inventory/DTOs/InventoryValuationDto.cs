namespace InventoryManagement.Application.Features.Inventory.DTOs;

public record InventoryValuationDto(
    decimal TotalInventoryValue,
    IEnumerable<ProductValuationDto> ValuePerProduct,
    string ValuationMethodUsed
);

public record ProductValuationDto(
    Guid ProductId,
    string ProductName,
    decimal TotalValue
);
