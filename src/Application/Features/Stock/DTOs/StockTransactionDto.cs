using System;

namespace InventoryManagement.Application.Features.Stock.DTOs;

public class StockTransactionDto
{
    public Guid TransactionId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public string TransactionType { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public DateTime TransactionDate { get; set; }
    public string Reference { get; set; } = string.Empty;
}
