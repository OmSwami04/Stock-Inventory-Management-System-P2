namespace InventoryManagement.Interfaces.Services;

public interface IInventoryAnalyticsService
{
    Task<int> GetLowStockAlertsCountAsync();
    Task<decimal> GetTotalInventoryValueAsync();
}
