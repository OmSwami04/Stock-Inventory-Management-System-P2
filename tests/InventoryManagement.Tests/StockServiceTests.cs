using Moq;
using InventoryManagement.Infrastructure.Services;
using InventoryManagement.Interfaces.Repositories;
using InventoryManagement.Interfaces;
using InventoryManagement.Interfaces.Pipelines;
using Microsoft.Extensions.Logging;
using AutoMapper;
using InventoryManagement.Application.Features.Stock.Commands;
using InventoryManagement.Application.Features.Stock.DTOs;
using InventoryManagement.Domain.Enums;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Shared.Exceptions;
using InventoryManagement.Interfaces.Services;
using Xunit;

namespace InventoryManagement.Tests;

public class StockServiceTests
{
    private readonly Mock<IStockTransactionRepository> _transactionRepoMock;
    private readonly Mock<IStockLevelRepository> _stockLevelRepoMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<IStockTransactionValidationPipeline> _validationPipelineMock;
    private readonly Mock<ILogger<StockService>> _loggerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IStockNotificationService> _notificationServiceMock;
    private readonly StockService _stockService;

    public StockServiceTests()
    {
        _transactionRepoMock = new Mock<IStockTransactionRepository>();
        _stockLevelRepoMock = new Mock<IStockLevelRepository>();
        _uowMock = new Mock<IUnitOfWork>();
        _validationPipelineMock = new Mock<IStockTransactionValidationPipeline>();
        _loggerMock = new Mock<ILogger<StockService>>();
        _mapperMock = new Mock<IMapper>();
        _notificationServiceMock = new Mock<IStockNotificationService>();

        _stockService = new StockService(
            _transactionRepoMock.Object,
            _stockLevelRepoMock.Object,
            _uowMock.Object,
            _validationPipelineMock.Object,
            _loggerMock.Object,
            _mapperMock.Object,
            _notificationServiceMock.Object);
    }

    [Fact]
    public async Task CreateStockTransaction_Should_BeginAndCommitTransaction()
    {
        // Arrange
        var command = new CreateStockTransactionCommand(
            Guid.NewGuid(), 
            Guid.NewGuid(), 
            TransactionType.Purchase, 
            10, 
            "REF123", 
            DateTime.UtcNow);

        _stockLevelRepoMock.Setup(x => x.GetByProductAndWarehouseAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(new StockLevel { QuantityOnHand = 0 });

        // Act
        await _stockService.CreateStockTransactionAsync(command.ProductId, command.WarehouseId, command.TransactionType.ToString(), command.Quantity, command.ReferenceNumber, command.TransactionDate);

        // Assert
        _uowMock.Verify(x => x.BeginTransactionAsync(), Times.Once);
        _uowMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateStockTransaction_Should_Rollback_On_Failure()
    {
        // Arrange
        var command = new CreateStockTransactionCommand(
            Guid.NewGuid(), 
            Guid.NewGuid(), 
            TransactionType.Purchase, 
            10, 
            "REF123", 
            DateTime.UtcNow);

        _stockLevelRepoMock.Setup(x => x.GetByProductAndWarehouseAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ThrowsAsync(new Exception("DB Error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _stockService.CreateStockTransactionAsync(command.ProductId, command.WarehouseId, command.TransactionType.ToString(), command.Quantity, command.ReferenceNumber, command.TransactionDate));
        _uowMock.Verify(x => x.RollbackAsync(), Times.Once);
    }

    [Fact]
    public async Task GetStockByProduct_ShouldReturnMappedStock()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var stockList = new List<StockLevel> { new StockLevel { ProductId = productId } };
        _stockLevelRepoMock.Setup(x => x.GetByProductIdAsync(productId)).ReturnsAsync(stockList);
        _mapperMock.Setup(m => m.Map<IEnumerable<StockLevelDto>>(stockList)).Returns(new List<StockLevelDto> { new StockLevelDto() });

        // Act
        var result = await _stockService.GetStockByProductAsync(productId);

        // Assert
        Assert.Single(result);
    }

    [Fact]
    public async Task TransferStock_Should_DecreaseSourceAndIncreaseDestination()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var fromWarId = Guid.NewGuid();
        var toWarId = Guid.NewGuid();
        var sourceStock = new StockLevel { ProductId = productId, WarehouseId = fromWarId, QuantityOnHand = 100 };
        var destStock = new StockLevel { ProductId = productId, WarehouseId = toWarId, QuantityOnHand = 50 };

        _stockLevelRepoMock.Setup(x => x.GetByProductAndWarehouseAsync(productId, fromWarId)).ReturnsAsync(sourceStock);
        _stockLevelRepoMock.Setup(x => x.GetByProductAndWarehouseAsync(productId, toWarId)).ReturnsAsync(destStock);
        _stockLevelRepoMock.Setup(x => x.GetByProductIdAsync(productId)).ReturnsAsync(new List<StockLevel> { sourceStock, destStock });

        // Act
        await _stockService.TransferStockAsync(productId, fromWarId, toWarId, 20, "TRF123");

        // Assert
        Assert.Equal(80, sourceStock.QuantityOnHand);
        Assert.Equal(70, destStock.QuantityOnHand);
        _stockLevelRepoMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        _transactionRepoMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task TransferStock_Should_Throw_If_Insufficient_Stock()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var fromWarId = Guid.NewGuid();
        var toWarId = Guid.NewGuid();
        var sourceStock = new StockLevel { ProductId = productId, WarehouseId = fromWarId, QuantityOnHand = 10 };

        _stockLevelRepoMock.Setup(x => x.GetByProductAndWarehouseAsync(productId, fromWarId)).ReturnsAsync(sourceStock);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _stockService.TransferStockAsync(productId, fromWarId, toWarId, 20, "TRF123"));
    }

    [Fact]
    public async Task GetAllStock_ShouldReturnMappedList()
    {
        // Arrange
        var stocks = new List<StockLevel> { new StockLevel() };
        _stockLevelRepoMock.Setup(x => x.GetAllAsync()).ReturnsAsync(stocks);
        _mapperMock.Setup(m => m.Map<IEnumerable<StockLevelDto>>(stocks)).Returns(new List<StockLevelDto> { new StockLevelDto() });

        // Act
        var result = await _stockService.GetAllStockAsync();

        // Assert
        Assert.Single(result);
    }

    [Fact]
    public async Task GetStockByWarehouse_ShouldReturnMappedList()
    {
        // Arrange
        var warId = Guid.NewGuid();
        var stocks = new List<StockLevel> { new StockLevel { WarehouseId = warId } };
        _stockLevelRepoMock.Setup(x => x.GetByWarehouseIdAsync(warId)).ReturnsAsync(stocks);
        _mapperMock.Setup(m => m.Map<IEnumerable<StockLevelDto>>(stocks)).Returns(new List<StockLevelDto> { new StockLevelDto() });

        // Act
        var result = await _stockService.GetStockByWarehouseAsync(warId);

        // Assert
        Assert.Single(result);
    }
}
