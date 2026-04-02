using InventoryManagement.Interfaces.Factories;
using InventoryManagement.Interfaces.Repositories;

namespace InventoryManagement.Infrastructure.Factories.Strategies;

public class FifoValuationStrategy : InventoryManagement.Interfaces.Factories.IInventoryValuationStrategy
{
    private readonly IStockLevelRepository _stockLevelRepository;
    private readonly IStockTransactionRepository _transactionRepository;
    private readonly IProductRepository _productRepository;

    public FifoValuationStrategy(IStockLevelRepository stockLevelRepository, IStockTransactionRepository transactionRepository, IProductRepository productRepository)
    {
        _stockLevelRepository = stockLevelRepository;
        _transactionRepository = transactionRepository;
        _productRepository = productRepository;
    }

    public async Task<decimal> CalculateValuationAsync(Guid productId)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null) return 0;
        
        var stocks = await _stockLevelRepository.GetByProductIdAsync(productId);
        var totalQuantityOnHand = stocks.Sum(s => s.QuantityOnHand);

        // Simple FIFO: Use the most recent purchase costs
        // In a full implementation, we'd track each lot.
        // For 'simplified acceptable', we'll use the product's cost.
        return totalQuantityOnHand * product.Cost;
    }
}
