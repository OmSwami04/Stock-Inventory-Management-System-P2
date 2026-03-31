using MediatR;

namespace InventoryManagement.Application.Features.Products.Commands;

public record DeleteProductCommand(Guid ProductId) : IRequest<Unit>;
