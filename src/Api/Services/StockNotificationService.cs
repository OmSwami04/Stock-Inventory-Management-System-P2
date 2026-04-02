using InventoryManagement.Api.Hubs;
using InventoryManagement.Interfaces.Services;
using Microsoft.AspNetCore.SignalR;

namespace InventoryManagement.Api.Services;

public class StockNotificationService : IStockNotificationService
{
    private readonly IHubContext<InventoryHub> _hubContext;

    public StockNotificationService(IHubContext<InventoryHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyStockUpdateAsync(Guid productId, string productName, int quantityOnHand, int reorderLevel)
    {
        // Broadcast stock update to all connected clients
        await _hubContext.Clients.All.SendAsync("ReceiveStockUpdate", new
        {
            ProductId = productId,
            ProductName = productName,
            QuantityOnHand = quantityOnHand,
            ReorderLevel = reorderLevel,
            IsLowStock = quantityOnHand <= reorderLevel
        });
    }
}
