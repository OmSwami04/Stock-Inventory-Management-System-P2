using Microsoft.AspNetCore.SignalR;

namespace InventoryManagement.Api.Hubs;

public class InventoryHub : Hub
{
    // Clients can subscribe to certain updates if needed, but for now we'll broadcast to all.
}
