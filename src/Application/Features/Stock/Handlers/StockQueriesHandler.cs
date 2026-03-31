using MediatR;
using AutoMapper;
using InventoryManagement.Application.Features.Stock.DTOs;
using InventoryManagement.Interfaces.Repositories;

namespace InventoryManagement.Application.Features.Stock.Handlers;

public class StockQueriesHandler : 
    IRequestHandler<Queries.GetAllStockQuery, IEnumerable<StockLevelDto>>,
    IRequestHandler<Queries.GetStockByProductQuery, IEnumerable<StockLevelDto>>,
    IRequestHandler<Queries.GetStockByWarehouseQuery, IEnumerable<StockLevelDto>>
{
    private readonly IStockLevelRepository _stockLevelRepository;
    private readonly IMapper _mapper;

    public StockQueriesHandler(IStockLevelRepository stockLevelRepository, IMapper mapper)
    {
        _stockLevelRepository = stockLevelRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<StockLevelDto>> Handle(Queries.GetAllStockQuery request, CancellationToken cancellationToken)
    {
        var stocks = await _stockLevelRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<StockLevelDto>>(stocks);
    }

    public async Task<IEnumerable<StockLevelDto>> Handle(Queries.GetStockByProductQuery request, CancellationToken cancellationToken)
    {
        var stocks = await _stockLevelRepository.GetByProductIdAsync(request.ProductId);
        return _mapper.Map<IEnumerable<StockLevelDto>>(stocks);
    }

    public async Task<IEnumerable<StockLevelDto>> Handle(Queries.GetStockByWarehouseQuery request, CancellationToken cancellationToken)
    {
        var stocks = await _stockLevelRepository.GetByWarehouseIdAsync(request.WarehouseId);
        return _mapper.Map<IEnumerable<StockLevelDto>>(stocks);
    }
}
