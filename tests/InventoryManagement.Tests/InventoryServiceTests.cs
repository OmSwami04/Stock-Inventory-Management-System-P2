using Moq;
using InventoryManagement.Infrastructure.Services;
using InventoryManagement.Interfaces.Repositories;
using InventoryManagement.Interfaces.Factories;
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
    private readonly Mock<ILogger<InventoryService>> _loggerMock;
    private readonly InventoryService _inventoryService;

    public InventoryServiceTests()
    {
        _valuationFactoryMock = new Mock<IInventoryValuationFactory>();
        _productRepoMock = new Mock<IProductRepository>();
        _stockLevelRepoMock = new Mock<IStockLevelRepository>();
        _loggerMock = new Mock<ILogger<InventoryService>>();

        _inventoryService = new InventoryService(
            _valuationFactoryMock.Object,
            _productRepoMock.Object,
            _stockLevelRepoMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task GetLowStockCount_Should_Return_Count()
    {
        // Arrange
        var stocks = new List<StockLevel> 
        { 
            new StockLevel { QuantityOnHand = 5, Product = new Product { ReorderLevel = 10 } },
            new StockLevel { QuantityOnHand = 15, Product = new Product { ReorderLevel = 10 } }
        };
        _stockLevelRepoMock.Setup(x => x.GetAllAsync()).ReturnsAsync(stocks);

        // Act
        var result = await _inventoryService.GetLowStockAlertsCountAsync();

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    public async Task GetTotalValue_Should_Return_CalculatedValue()
    {
        // Arrange
        var stocks = new List<StockLevel> 
        { 
            new StockLevel { ProductId = Guid.Parse("00000000-0000-0000-0000-000000000001"), QuantityOnHand = 10 },
            new StockLevel { ProductId = Guid.Parse("00000000-0000-0000-0000-000000000002"), QuantityOnHand = 5 }
        };
        var products = new List<Product>
        {
            new Product { ProductId = Guid.Parse("00000000-0000-0000-0000-000000000001"), Cost = 100 },
            new Product { ProductId = Guid.Parse("00000000-0000-0000-0000-000000000002"), Cost = 200 }
        };
        _stockLevelRepoMock.Setup(x => x.GetAllAsync()).ReturnsAsync(stocks);
        _productRepoMock.Setup(x => x.GetAllAsync()).ReturnsAsync(products);

        // Act
        var result = await _inventoryService.GetTotalInventoryValueAsync();

        // Assert
        Assert.Equal(2000m, result); // (10 * 100) + (5 * 200) = 1000 + 1000 = 2000
    }
}
