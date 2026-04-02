using Moq;
using InventoryManagement.Infrastructure.Services;
using InventoryManagement.Interfaces.Repositories;
using InventoryManagement.Interfaces;
using AutoMapper;
using InventoryManagement.Application.Features.Products.Commands;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Shared.Exceptions;
using Xunit;

namespace InventoryManagement.Tests;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _productRepoMock;
    private readonly Mock<IStockLevelRepository> _stockLevelRepoMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _productRepoMock = new Mock<IProductRepository>();
        _stockLevelRepoMock = new Mock<IStockLevelRepository>();
        _uowMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();

        _productService = new ProductService(
            _productRepoMock.Object,
            _stockLevelRepoMock.Object,
            _uowMock.Object,
            _mapperMock.Object);
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
        _productRepoMock.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(new Product());
        _stockLevelRepoMock.Setup(x => x.GetByProductIdAsync(productId))
            .ReturnsAsync(new List<StockLevel> { new StockLevel { QuantityOnHand = 0 } });

        // Act
        await _productService.DeleteProductAsync(productId);

        // Assert
        _productRepoMock.Verify(x => x.Delete(It.IsAny<Product>()), Times.Once);
        _uowMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }
}
