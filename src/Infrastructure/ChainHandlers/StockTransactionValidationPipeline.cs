using InventoryManagement.Interfaces.Pipelines;
using InventoryManagement.Interfaces.Repositories;
using InventoryManagement.Shared.Exceptions;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Infrastructure.ChainHandlers;

public class StockTransactionValidationPipeline : IStockTransactionValidationPipeline
{
    private readonly IProductRepository _productRepository;
    private readonly IGenericRepository<Warehouse> _warehouseRepository;
    private readonly IStockLevelRepository _stockLevelRepository;

    public StockTransactionValidationPipeline(
        IProductRepository productRepository,
        IGenericRepository<Warehouse> warehouseRepository,
        IStockLevelRepository stockLevelRepository)
    {
        _productRepository = productRepository;
        _warehouseRepository = warehouseRepository;
        _stockLevelRepository = stockLevelRepository;
    }

    public async Task ValidateAsync(TransactionRequestContext context)
    {
        // Handler 1: Validate Product Exists
        context.Product = await _productRepository.GetByIdAsync(context.ProductId);
        if (context.Product == null)
            throw new NotFoundException($"Product with ID {context.ProductId} not found.");

        // Handler 2: Validate Warehouse Exists
        context.Warehouse = await _warehouseRepository.GetByIdAsync(context.WarehouseId);
        if (context.Warehouse == null)
            throw new NotFoundException($"Warehouse with ID {context.WarehouseId} not found.");

        // Handler 3: Validate Transaction Type and Quantity
        if (!Enum.IsDefined(typeof(Domain.Enums.TransactionType), context.TransactionType))
            throw new BadRequestException($"Invalid transaction type: {context.TransactionType}");
        if (context.Quantity <= 0)
            throw new BadRequestException("Quantity must be greater than zero.");

        // Handler 4: Validate Stock Availability for Sales
        context.Stock = await _stockLevelRepository.GetByProductAndWarehouseAsync(context.ProductId, context.WarehouseId);
        if (context.TransactionType == Domain.Enums.TransactionType.Sale)
        {
            if (context.Stock == null || context.Stock.QuantityOnHand < context.Quantity)
                throw new BadRequestException($"Insufficient stock for product {context.ProductId} in warehouse {context.WarehouseId}.");
        }
    }
}
