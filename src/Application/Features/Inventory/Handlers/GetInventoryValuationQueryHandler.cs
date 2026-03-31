using MediatR;
using InventoryManagement.Application.Features.Inventory.DTOs;
using InventoryManagement.Application.Features.Inventory.Queries;
using InventoryManagement.Interfaces.Factories;
using InventoryManagement.Interfaces.Repositories;

namespace InventoryManagement.Application.Features.Inventory.Handlers;

public class GetInventoryValuationQueryHandler : IRequestHandler<GetInventoryValuationQuery, InventoryValuationDto>
{
    private readonly IInventoryValuationFactory _valuationFactory;
    private readonly IProductRepository _productRepository;

    public GetInventoryValuationQueryHandler(IInventoryValuationFactory valuationFactory, IProductRepository productRepository)
    {
        _valuationFactory = valuationFactory;
        _productRepository = productRepository;
    }

    public async Task<InventoryValuationDto> Handle(GetInventoryValuationQuery request, CancellationToken cancellationToken)
    {
        var strategy = _valuationFactory.GetStrategy(request.Method);
        var products = await _productRepository.GetAllAsync();

        var productValuations = new List<ProductValuationDto>();
        decimal totalValue = 0;

        foreach (var product in products)
        {
            var value = await strategy.CalculateValuationAsync(product.ProductId);
            productValuations.Add(new ProductValuationDto(product.ProductId, product.ProductName, value));
            totalValue += value;
        }

        return new InventoryValuationDto(totalValue, productValuations, request.Method.ToString());
    }
}
