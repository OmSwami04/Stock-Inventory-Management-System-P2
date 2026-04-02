namespace InventoryManagement.Interfaces.Services;

public interface IStockNotificationService
{
    Task NotifyStockUpdateAsync(Guid productId, string productName, int quantityOnHand, int reorderLevel);
}
