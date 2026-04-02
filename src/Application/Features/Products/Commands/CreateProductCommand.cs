using MediatR;

namespace InventoryManagement.Application.Features.Products.Commands;

public record CreateProductCommand(
    string SKU,
    string ProductName,
    string Description,
    Guid CategoryId,
    string UnitOfMeasure,
    decimal Cost,
    decimal ListPrice,
    int ReorderLevel,
    int SafetyStock
) : IRequest<Guid>;
