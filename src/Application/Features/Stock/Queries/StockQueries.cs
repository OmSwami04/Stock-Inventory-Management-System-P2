using MediatR;
using InventoryManagement.Application.Features.Stock.DTOs;

namespace InventoryManagement.Application.Features.Stock.Queries;

public record GetAllStockQuery() : IRequest<IEnumerable<StockLevelDto>>;
public record GetStockByProductQuery(Guid ProductId) : IRequest<IEnumerable<StockLevelDto>>;
public record GetStockByWarehouseQuery(Guid WarehouseId) : IRequest<IEnumerable<StockLevelDto>>;
