using MediatR;

namespace InventoryManagement.Application.Features.Products.Commands;

public record UpdateProductCommand(
    Guid ProductId,
    string ProductName,
    string Description,
    Guid CategoryId,
    string UnitOfMeasure,
    decimal Cost,
    decimal ListPrice,
    int ReorderLevel,
    int SafetyStock,
    bool IsActive
) : IRequest<Unit>;
