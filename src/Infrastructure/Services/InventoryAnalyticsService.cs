using InventoryManagement.Interfaces.Services;
using InventoryManagement.Interfaces.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryManagement.Infrastructure.Services;

public class InventoryAnalyticsService : IInventoryAnalyticsService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public InventoryAnalyticsService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task<int> GetLowStockAlertsCountAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var stockLevelRepository = scope.ServiceProvider.GetRequiredService<IStockLevelRepository>();
        var stocks = await stockLevelRepository.GetAllAsync();
        
        // Use the product's specific reorder level for accurate alerts
        return stocks.Count(s => s.Product != null && s.QuantityOnHand <= s.Product.ReorderLevel);
    }

    public async Task<decimal> GetTotalInventoryValueAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var stockLevelRepository = scope.ServiceProvider.GetRequiredService<IStockLevelRepository>();
        var productRepository = scope.ServiceProvider.GetRequiredService<IProductRepository>();
        
        var stocks = await stockLevelRepository.GetAllAsync();
        var products = await productRepository.GetAllAsync();
        
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
