using MediatR;
using InventoryManagement.Application.Features.Products.DTOs;

namespace InventoryManagement.Application.Features.Products.Queries;

public record GetProductByIdQuery(Guid ProductId) : IRequest<ProductDto>;
