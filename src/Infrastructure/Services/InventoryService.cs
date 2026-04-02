using InventoryManagement.Application.Features.Inventory.DTOs;
using InventoryManagement.Application.Features.Inventory.Queries;
using InventoryManagement.Interfaces.Factories;
using InventoryManagement.Interfaces.Repositories;
using InventoryManagement.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Infrastructure.Services;

public class InventoryService : IInventoryService
{
    private readonly IInventoryValuationFactory _valuationFactory;
    private readonly IProductRepository _productRepository;
    private readonly IStockLevelRepository _stockLevelRepository;
    private readonly ILogger<InventoryService> _logger;

    public InventoryService(
        IInventoryValuationFactory valuationFactory,
        IProductRepository productRepository,
        IStockLevelRepository stockLevelRepository,
        ILogger<InventoryService> logger)
    {
        _valuationFactory = valuationFactory;
        _productRepository = productRepository;
        _stockLevelRepository = stockLevelRepository;
        _logger = logger;
    }

    public async Task<object> GetInventoryValuationAsync(string method)
    {
        var valuationMethod = Enum.Parse<ValuationMethod>(method);
        var strategy = _valuationFactory.GetStrategy(valuationMethod);
        var products = await _productRepository.GetAllAsync();

        var productValuations = new List<ProductValuationDto>();
        decimal totalValue = 0;

        foreach (var product in products)
        {
            var value = await strategy.CalculateValuationAsync(product.ProductId);
            productValuations.Add(new ProductValuationDto(product.ProductId, product.ProductName, value));
            totalValue += value;
        }

        return new InventoryValuationDto(totalValue, productValuations, method);
    }

    public async Task<int> GetLowStockAlertsCountAsync()
    {
        var stocks = await _stockLevelRepository.GetAllAsync();
        // Use the product's specific reorder level for accurate alerts
        var count = stocks.Count(s => s.Product != null && s.QuantityOnHand <= s.Product.ReorderLevel);
        return count;
    }

    public async Task<decimal> GetTotalInventoryValueAsync()
    {
        var stocks = await _stockLevelRepository.GetAllAsync();
        var products = await _productRepository.GetAllAsync();
        
        decimal totalValue = 0;
        foreach(var stock in stocks)
        {
            var product = products.FirstOrDefault(p => p.ProductId == stock.ProductId);
            if (product != null)
            {
                totalValue += stock.QuantityOnHand * product.Cost;
            }
        }
        return totalValue;
    }
}
