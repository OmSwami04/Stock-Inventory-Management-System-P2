using Moq;
using InventoryManagement.Infrastructure.Services;
using InventoryManagement.Interfaces.Repositories;
using InventoryManagement.Interfaces;
using AutoMapper;
using InventoryManagement.Application.Features.Products.Commands;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Shared.Exceptions;
using InventoryManagement.Application.Features.Products.DTOs;
using InventoryManagement.Interfaces.Services;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Xunit;

namespace InventoryManagement.Tests;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _productRepoMock;
    private readonly Mock<IStockLevelRepository> _stockLevelRepoMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IStockNotificationService> _notificationServiceMock;
    private readonly Mock<IDistributedCache> _cacheMock;
    private readonly Mock<ILogger<ProductService>> _loggerMock;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _productRepoMock = new Mock<IProductRepository>();
        _stockLevelRepoMock = new Mock<IStockLevelRepository>();
        _uowMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _notificationServiceMock = new Mock<IStockNotificationService>();
        _cacheMock = new Mock<IDistributedCache>();
        _loggerMock = new Mock<ILogger<ProductService>>();

        _productService = new ProductService(
            _productRepoMock.Object,
            _stockLevelRepoMock.Object,
            _uowMock.Object,
            _mapperMock.Object,
            _notificationServiceMock.Object,
            _cacheMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task CreateProduct_Should_Save()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();

        // Act
        await _productService.CreateProductAsync("SKU1", "Prod1", "Desc", categoryId, "pcs", 10, 20, 5, 2);

        // Assert
        _productRepoMock.Verify(x => x.AddAsync(It.IsAny<Product>()), Times.Once);
        _uowMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteProduct_Should_Throw_If_Stock_Exists()
    {
        // Arrange
        var productId = Guid.NewGuid();
        _productRepoMock.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(new Product());
        _stockLevelRepoMock.Setup(x => x.GetByProductIdAsync(productId))
            .ReturnsAsync(new List<StockLevel> { new StockLevel { QuantityOnHand = 5 } });

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _productService.DeleteProductAsync(productId));
    }

    [Fact]
    public async Task DeleteProduct_Should_Succeed_If_No_Stock()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product { ProductId = productId, ProductName = "Test" };
        _productRepoMock.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);
        _stockLevelRepoMock.Setup(x => x.GetByProductIdAsync(productId))
            .ReturnsAsync(new List<StockLevel> { new StockLevel { QuantityOnHand = 0 } });

        // Act
        await _productService.DeleteProductAsync(productId);

        // Assert
        _productRepoMock.Verify(x => x.Delete(It.IsAny<Product>()), Times.Once);
        _uowMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateProduct_Should_UpdateAndSave()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product { ProductId = productId, ProductName = "OldName" };
        _productRepoMock.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);

        // Act
        await _productService.UpdateProductAsync(productId, "NewName", "NewDesc", Guid.NewGuid(), "pcs", 10, 20, 5, 2, true);

        // Assert
        Assert.Equal("NewName", product.ProductName);
        _productRepoMock.Verify(x => x.Update(product), Times.Once);
        _uowMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetProductById_Should_Return_Product()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product { ProductId = productId, ProductName = "Test" };
        _productRepoMock.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);
        _mapperMock.Setup(m => m.Map<ProductDto>(product)).Returns(new ProductDto { ProductId = productId, ProductName = "Test" });

        // Act
        var result = await _productService.GetProductByIdAsync(productId);

        // Assert
        Assert.NotNull(result);
        var productDto = Assert.IsType<ProductDto>(result);
        Assert.Equal(productId, productDto.ProductId);
    }

    [Fact]
    public async Task UpdateProduct_Should_Invalidate_Cache()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product { ProductId = productId, ProductName = "Test" };
        _productRepoMock.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);

        // Act
        await _productService.UpdateProductAsync(productId, "New", "Desc", Guid.NewGuid(), "pcs", 10, 20, 5, 2, true);

        // Assert
        _cacheMock.Verify(x => x.RemoveAsync($"Product_{productId}", default), Times.Once);
    }

    [Fact]
    public async Task DeleteProduct_Should_Invalidate_Cache()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product { ProductId = productId, ProductName = "Test" };
        _productRepoMock.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);
        _stockLevelRepoMock.Setup(x => x.GetByProductIdAsync(productId)).ReturnsAsync(new List<StockLevel>());

        // Act
        await _productService.DeleteProductAsync(productId);

        // Assert
        _cacheMock.Verify(x => x.RemoveAsync($"Product_{productId}", default), Times.Once);
    }
}
