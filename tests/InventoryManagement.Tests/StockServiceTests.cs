using Moq;
using InventoryManagement.Infrastructure.Services;
using InventoryManagement.Interfaces.Repositories;
using InventoryManagement.Interfaces;
using InventoryManagement.Interfaces.Pipelines;
using Microsoft.Extensions.Logging;
using AutoMapper;
using InventoryManagement.Application.Features.Stock.Commands;
using InventoryManagement.Domain.Enums;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Shared.Exceptions;
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
    private readonly StockService _stockService;

    public StockServiceTests()
    {
        _transactionRepoMock = new Mock<IStockTransactionRepository>();
        _stockLevelRepoMock = new Mock<IStockLevelRepository>();
        _uowMock = new Mock<IUnitOfWork>();
        _validationPipelineMock = new Mock<IStockTransactionValidationPipeline>();
        _loggerMock = new Mock<ILogger<StockService>>();
        _mapperMock = new Mock<IMapper>();

        _stockService = new StockService(
            _transactionRepoMock.Object,
            _stockLevelRepoMock.Object,
            _uowMock.Object,
            _validationPipelineMock.Object,
            _loggerMock.Object,
            _mapperMock.Object);
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
    public async Task GetStockByWarehouse_ShouldReturnMappedStock()
    {
        // Arrange
        var warehouseId = Guid.NewGuid();
        var stockList = new List<StockLevel> { new StockLevel { WarehouseId = warehouseId } };
        _stockLevelRepoMock.Setup(x => x.GetByWarehouseIdAsync(warehouseId)).ReturnsAsync(stockList);
        _mapperMock.Setup(m => m.Map<IEnumerable<StockLevelDto>>(stockList)).Returns(new List<StockLevelDto> { new StockLevelDto() });

        // Act
        var result = await _stockService.GetStockByWarehouseAsync(warehouseId);

        // Assert
        Assert.Single(result);
    }

    [Fact]
    public async Task GetAllStock_ShouldReturnMappedStock()
    {
        // Arrange
        var stockList = new List<StockLevel> { new StockLevel(), new StockLevel() };
        _stockLevelRepoMock.Setup(x => x.GetAllAsync()).ReturnsAsync(stockList);
        _mapperMock.Setup(m => m.Map<IEnumerable<StockLevelDto>>(stockList)).Returns(new List<StockLevelDto> { new StockLevelDto(), new StockLevelDto() });

        // Act
        var result = await _stockService.GetAllStockAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }
}
