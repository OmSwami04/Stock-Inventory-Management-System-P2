using System;

namespace InventoryManagement.Application.Features.Stock.DTOs;

public class StockTransferRequest
{
    public Guid ProductId { get; set; }
    public Guid FromWarehouseId { get; set; }
    public Guid ToWarehouseId { get; set; }
    public int Quantity { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
}
