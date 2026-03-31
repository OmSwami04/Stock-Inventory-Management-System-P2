using InventoryManagement.Interfaces.Factories;
using InventoryManagement.Infrastructure.Factories.Strategies;
using InventoryManagement.Interfaces.Repositories;

namespace InventoryManagement.Infrastructure.Factories;

public class InventoryValuationFactory : IInventoryValuationFactory
{
    private readonly IStockLevelRepository _stockLevelRepository;
    private readonly IStockTransactionRepository _transactionRepository;
    private readonly IProductRepository _productRepository;

    public InventoryValuationFactory(
        IStockLevelRepository stockLevelRepository,
        IStockTransactionRepository transactionRepository,
        IProductRepository productRepository)
    {
        _stockLevelRepository = stockLevelRepository;
        _transactionRepository = transactionRepository;
        _productRepository = productRepository;
    }

    public Interfaces.Factories.IInventoryValuationStrategy GetStrategy(ValuationMethod method)
    {
        return method switch
        {
            ValuationMethod.FIFO => new FifoValuationStrategy(_stockLevelRepository, _transactionRepository, _productRepository),
            ValuationMethod.LIFO => new LifoValuationStrategy(_stockLevelRepository, _transactionRepository, _productRepository),
            ValuationMethod.WeightedAverage => new WeightedAverageStrategy(_stockLevelRepository, _productRepository),
            _ => throw new ArgumentException("Invalid valuation method", nameof(method))
        };
    }
}
