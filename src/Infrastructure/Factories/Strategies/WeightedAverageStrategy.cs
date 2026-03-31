using InventoryManagement.Interfaces.Factories;
using InventoryManagement.Interfaces.Repositories;

namespace InventoryManagement.Infrastructure.Factories.Strategies;

public class WeightedAverageStrategy : InventoryManagement.Interfaces.Factories.IInventoryValuationStrategy
{
    private readonly IStockLevelRepository _stockLevelRepository;
    private readonly IProductRepository _productRepository;

    public WeightedAverageStrategy(IStockLevelRepository stockLevelRepository, IProductRepository productRepository)
    {
        _stockLevelRepository = stockLevelRepository;
        _productRepository = productRepository;
    }

    public async Task<decimal> CalculateValuationAsync(Guid productId)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null) return 0;
        var stocks = await _stockLevelRepository.GetByProductIdAsync(productId);
        var totalQuantity = stocks.Sum(s => s.QuantityOnHand);
        return totalQuantity * product.Cost;
    }
}
