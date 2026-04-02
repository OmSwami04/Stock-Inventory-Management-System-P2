using Moq;
using InventoryManagement.Infrastructure.Services;
using InventoryManagement.Interfaces.Repositories;
using InventoryManagement.Interfaces.Factories;
using InventoryManagement.Interfaces.Caching;
using Microsoft.Extensions.Logging;
using InventoryManagement.Application.Features.Inventory.Queries;
using InventoryManagement.Application.Features.Inventory.DTOs;
using InventoryManagement.Domain.Entities;
using Xunit;

namespace InventoryManagement.Tests;

public class InventoryServiceTests
{
    private readonly Mock<IInventoryValuationFactory> _valuationFactoryMock;
    private readonly Mock<IProductRepository> _productRepoMock;
    private readonly Mock<IStockLevelRepository> _stockLevelRepoMock;
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly Mock<ILogger<InventoryService>> _loggerMock;
    private readonly InventoryService _inventoryService;

    public InventoryServiceTests()
    {
        _valuationFactoryMock = new Mock<IInventoryValuationFactory>();
        _productRepoMock = new Mock<IProductRepository>();
        _stockLevelRepoMock = new Mock<IStockLevelRepository>();
        _cacheServiceMock = new Mock<ICacheService>();
        _loggerMock = new Mock<ILogger<InventoryService>>();

        _inventoryService = new InventoryService(
            _valuationFactoryMock.Object,
            _productRepoMock.Object,
            _stockLevelRepoMock.Object,
            _cacheServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task GetLowStockCount_Should_Return_Cached_If_Exists()
    {
        // Arrange
        _cacheServiceMock.Setup(x => x.GetAsync<int?>("Inventory_LowStockCount")).ReturnsAsync(5);

        // Act
        var result = await _inventoryService.GetLowStockAlertsCountAsync();

        // Assert
        Assert.Equal(5, result);
        _stockLevelRepoMock.Verify(x => x.GetAllAsync(), Times.Never);
    }

    [Fact]
    public async Task GetTotalValue_Should_Return_Cached_If_Exists()
    {
        // Arrange
        _cacheServiceMock.Setup(x => x.GetAsync<decimal?>("Inventory_TotalValue")).ReturnsAsync(100.5m);

        // Act
        var result = await _inventoryService.GetTotalInventoryValueAsync();

        // Assert
        Assert.Equal(100.5m, result);
    }
}
