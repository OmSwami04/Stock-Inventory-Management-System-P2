using MediatR;
using InventoryManagement.Application.Features.Stock.Commands;
using InventoryManagement.Interfaces.Pipelines;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Domain.Enums;
using InventoryManagement.Interfaces;
using InventoryManagement.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Features.Stock.Handlers;

public class CreateStockTransactionCommandHandler : IRequestHandler<CreateStockTransactionCommand, Guid>
{
    private readonly IStockTransactionRepository _transactionRepository;
    private readonly IStockLevelRepository _stockLevelRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStockTransactionValidationPipeline _validationPipeline;
    private readonly ILogger<CreateStockTransactionCommandHandler> _logger;

    public CreateStockTransactionCommandHandler(
        IStockTransactionRepository transactionRepository,
        IStockLevelRepository stockLevelRepository,
        IUnitOfWork unitOfWork,
        IStockTransactionValidationPipeline validationPipeline,
        ILogger<CreateStockTransactionCommandHandler> logger)
    {
        _transactionRepository = transactionRepository;
        _stockLevelRepository = stockLevelRepository;
        _unitOfWork = unitOfWork;
        _validationPipeline = validationPipeline;
        _logger = logger;
    }

    public async Task<Guid> Handle(CreateStockTransactionCommand request, CancellationToken cancellationToken)
    {
        var requestContext = new TransactionRequestContext
        {
            ProductId = request.ProductId,
            WarehouseId = request.WarehouseId,
            Quantity = request.Quantity,
            TransactionType = request.TransactionType
        };

        // 1. Run Validation Pipeline
        await _validationPipeline.ValidateAsync(requestContext);

        // 2. Begin atomic Transaction
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var stock = await _stockLevelRepository.GetByProductAndWarehouseAsync(request.ProductId, request.WarehouseId);

            if (stock == null)
            {
                stock = new StockLevel
                {
                    StockLevelId = Guid.NewGuid(),
                    ProductId = request.ProductId,
                    WarehouseId = request.WarehouseId,
                    QuantityOnHand = 0,
                    ReorderLevel = 0,
                    SafetyStock = 0
                };
                await _stockLevelRepository.AddAsync(stock);
            }

            // Compute quantity delta (keep enum for logic, DB stores as string via EF config if needed)
            int quantityChange = request.TransactionType switch
            {
                TransactionType.Purchase => request.Quantity,
                TransactionType.Sale => -request.Quantity,
                TransactionType.Adjustment => request.Quantity,
                TransactionType.Return => request.Quantity,
                TransactionType.Transfer => -request.Quantity,
                _ => request.Quantity
            };

            stock.QuantityOnHand += quantityChange;
            _stockLevelRepository.Update(stock);

            var transaction = new StockTransaction
            {
                TransactionId = Guid.NewGuid(),
                ProductId = request.ProductId,
                WarehouseId = request.WarehouseId,
                TransactionType = request.TransactionType.ToString(),
                Quantity = request.Quantity,
                Reference = request.ReferenceNumber, // Map from command
                TransactionDate = request.TransactionDate != default ? request.TransactionDate : DateTime.UtcNow
            };

            await _transactionRepository.AddAsync(transaction);

            // 4. Commit Unit of Work
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Processed stock transaction {TransactionId} for Product {ProductId}", transaction.TransactionId, request.ProductId);

            return transaction.TransactionId;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            _logger.LogError(ex, "Failed to process stock transaction for Product {ProductId}", request.ProductId);
            throw;
        }
    }
}
