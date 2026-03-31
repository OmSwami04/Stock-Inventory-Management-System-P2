using MediatR;
using InventoryManagement.Application.Features.Products.DTOs;

namespace InventoryManagement.Application.Features.Products.Queries;

public record GetAllProductsQuery(int PageNumber = 1, int PageSize = 10) : IRequest<PagedResponse<ProductDto>>;

public record PagedResponse<T>(IEnumerable<T> Data, int TotalCount, int PageNumber, int PageSize);
