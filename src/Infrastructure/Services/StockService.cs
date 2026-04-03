using InventoryManagement.Application.Features.Stock.Commands;
using InventoryManagement.Application.Features.Stock.DTOs;
using InventoryManagement.Application.Features.Stock.Queries;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Domain.Enums;
using InventoryManagement.Interfaces;
using InventoryManagement.Interfaces.Pipelines;
using InventoryManagement.Interfaces.Repositories;
using InventoryManagement.Interfaces.Services;
using Microsoft.Extensions.Logging;
using AutoMapper;

using InventoryManagement.Shared.Exceptions;

namespace InventoryManagement.Infrastructure.Services;

public class StockService : IStockService
{
    private readonly IStockTransactionRepository _transactionRepository;
    private readonly IStockLevelRepository _stockLevelRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStockTransactionValidationPipeline _validationPipeline;
    private readonly ILogger<StockService> _logger;
    private readonly IMapper _mapper;
    private readonly IStockNotificationService _notificationService;

    public StockService(
        IStockTransactionRepository transactionRepository,
        IStockLevelRepository stockLevelRepository,
        IUnitOfWork unitOfWork,
        IStockTransactionValidationPipeline validationPipeline,
        ILogger<StockService> logger,
        IMapper mapper,
        IStockNotificationService notificationService)
    {
        _transactionRepository = transactionRepository;
        _stockLevelRepository = stockLevelRepository;
        _unitOfWork = unitOfWork;
        _validationPipeline = validationPipeline;
        _logger = logger;
        _mapper = mapper;
        _notificationService = notificationService;
    }

    public async Task<Guid> CreateStockTransactionAsync(Guid productId, Guid warehouseId, string transactionType, int quantity, string referenceNumber, DateTime transactionDate)
    {
        TransactionType type;
        if (int.TryParse(transactionType, out int enumValue))
        {
            type = (TransactionType)enumValue;
        }
        else if (!Enum.TryParse<TransactionType>(transactionType, true, out type))
        {
            throw new BadRequestException($"Invalid transaction type: {transactionType}");
        }

        var requestContext = new TransactionRequestContext
        {
            ProductId = productId,
            WarehouseId = warehouseId,
            Quantity = quantity,
            TransactionType = type
        };

        // 1. Run Validation Pipeline
        await _validationPipeline.ValidateAsync(requestContext);

        // 2. Begin atomic Transaction
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var stock = await _stockLevelRepository.GetByProductAndWarehouseAsync(productId, warehouseId);

            if (stock == null)
            {
                stock = new StockLevel
                {
                    StockLevelId = Guid.NewGuid(),
                    ProductId = productId,
                    WarehouseId = warehouseId,
                    QuantityOnHand = 0,
                    ReorderLevel = 0,
                    SafetyStock = 0
                };
                await _stockLevelRepository.AddAsync(stock);
            }

            int quantityChange = type switch
            {
                TransactionType.Purchase => quantity,
                TransactionType.Sale => -quantity,
                TransactionType.Adjustment => quantity,
                TransactionType.Return => quantity,
                TransactionType.Transfer => -quantity,
                _ => quantity
            };

            stock.QuantityOnHand += quantityChange;
            // _stockLevelRepository.Update(stock); // Redundant if tracked, and potentially problematic if related entities are tracked but not updated.

            var transaction = new StockTransaction
            {
                TransactionId = Guid.NewGuid(),
                ProductId = productId,
                WarehouseId = warehouseId,
                TransactionType = type.ToString(),
                Quantity = quantity,
                Reference = string.IsNullOrWhiteSpace(referenceNumber) ? "N/A" : referenceNumber,
                TransactionDate = transactionDate != default && transactionDate != DateTime.MinValue ? transactionDate : DateTime.UtcNow
            };

            await _transactionRepository.AddAsync(transaction);

            await _unitOfWork.CommitAsync();

            // Notify real-time update
            if (stock.Product != null)
            {
                await _notificationService.NotifyStockUpdateAsync(productId, stock.Product.ProductName, stock.QuantityOnHand, stock.Product.ReorderLevel);
            }

            _logger.LogInformation("Processed stock transaction {TransactionId} for Product {ProductId}", transaction.TransactionId, productId);

            return transaction.TransactionId;
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task<IEnumerable<object>> GetAllStockAsync()
    {
        var stock = await _stockLevelRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<StockLevelDto>>(stock);
    }

    public async Task<IEnumerable<object>> GetStockByProductAsync(Guid productId)
    {
        var stock = await _stockLevelRepository.GetByProductIdAsync(productId);
        return _mapper.Map<IEnumerable<StockLevelDto>>(stock);
    }

    public async Task<IEnumerable<object>> GetStockByWarehouseAsync(Guid warehouseId)
    {
        var stock = await _stockLevelRepository.GetByWarehouseIdAsync(warehouseId);
        return _mapper.Map<IEnumerable<StockLevelDto>>(stock);
    }

    public async Task<IEnumerable<object>> GetStockTransactionsAsync()
    {
        var transactions = await _transactionRepository.GetAllWithDetailsAsync();
        return _mapper.Map<IEnumerable<StockTransactionDto>>(transactions);
    }

    public async Task TransferStockAsync(Guid productId, Guid fromWarehouseId, Guid toWarehouseId, int quantity, string referenceNumber)
    {
        if (fromWarehouseId == toWarehouseId)
            throw new InvalidOperationException("Source and destination warehouses cannot be the same.");

        if (quantity <= 0)
            throw new InvalidOperationException("Transfer quantity must be greater than zero.");

        // 1. Check source stock
        var sourceStock = await _stockLevelRepository.GetByProductAndWarehouseAsync(productId, fromWarehouseId);
        if (sourceStock == null || sourceStock.QuantityOnHand < quantity)
            throw new InvalidOperationException("Insufficient stock in the source warehouse.");

        // 2. Decrease source stock
        sourceStock.QuantityOnHand -= quantity;
        _stockLevelRepository.Update(sourceStock);

        // 3. Increase destination stock
        var destinationStock = await _stockLevelRepository.GetByProductAndWarehouseAsync(productId, toWarehouseId);
        if (destinationStock == null)
        {
            destinationStock = new StockLevel
            {
                StockLevelId = Guid.NewGuid(),
                ProductId = productId,
                WarehouseId = toWarehouseId,
                QuantityOnHand = quantity,
                ReorderLevel = sourceStock.ReorderLevel, // Inherit reorder level
                SafetyStock = sourceStock.SafetyStock   // Inherit safety stock
            };
            await _stockLevelRepository.AddAsync(destinationStock);
        }
        else
        {
            destinationStock.QuantityOnHand += quantity;
            _stockLevelRepository.Update(destinationStock);
        }

        // 4. Record transactions
        var outTransaction = new StockTransaction
        {
            TransactionId = Guid.NewGuid(),
            ProductId = productId,
            WarehouseId = fromWarehouseId,
            TransactionType = "Transfer Out",
            Quantity = -quantity,
            TransactionDate = DateTime.UtcNow,
            Reference = referenceNumber
        };

        var inTransaction = new StockTransaction
        {
            TransactionId = Guid.NewGuid(),
            ProductId = productId,
            WarehouseId = toWarehouseId,
            TransactionType = "Transfer In",
            Quantity = quantity,
            TransactionDate = DateTime.UtcNow,
            Reference = referenceNumber
        };

        await _transactionRepository.AddAsync(outTransaction);
        await _transactionRepository.AddAsync(inTransaction);

        await _stockLevelRepository.SaveChangesAsync();
        await _transactionRepository.SaveChangesAsync();

        // Notify real-time update
        var product = await _stockLevelRepository.GetByProductIdAsync(productId);
        var productInfo = product.FirstOrDefault()?.Product;
        if (productInfo != null)
        {
            var sourceStockLevel = sourceStock.QuantityOnHand;
            var destStockLevel = destinationStock.QuantityOnHand;
            
            await _notificationService.NotifyStockUpdateAsync(productId, productInfo.ProductName, sourceStockLevel, productInfo.ReorderLevel);
            await _notificationService.NotifyStockUpdateAsync(productId, productInfo.ProductName, destStockLevel, productInfo.ReorderLevel);
        }
    }
}
