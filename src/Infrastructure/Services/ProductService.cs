using AutoMapper;
using InventoryManagement.Application.Features.Products.Commands;
using InventoryManagement.Application.Features.Products.DTOs;
using InventoryManagement.Application.Features.Products.Queries;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Interfaces;
using InventoryManagement.Interfaces.Repositories;
using InventoryManagement.Interfaces.Services;
using InventoryManagement.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IStockLevelRepository _stockLevelRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProductService(
        IProductRepository productRepository, 
        IStockLevelRepository stockLevelRepository, 
        IUnitOfWork unitOfWork, 
        IMapper mapper)
    {
        _productRepository = productRepository;
        _stockLevelRepository = stockLevelRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> CreateProductAsync(string sku, string productName, string description, Guid categoryId, string unitOfMeasure, decimal cost, decimal listPrice, int reorderLevel, int safetyStock)
    {
        var product = new Product
        {
            ProductId = Guid.NewGuid(),
            SKU = sku,
            ProductName = productName,
            Description = description,
            CategoryId = categoryId,
            UnitOfMeasure = unitOfMeasure,
            Cost = cost,
            ListPrice = listPrice,
            ReorderLevel = reorderLevel,
            SafetyStock = safetyStock,
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };

        await _productRepository.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();

        return product.ProductId;
    }

    public async Task UpdateProductAsync(Guid productId, string productName, string description, Guid categoryId, string unitOfMeasure, decimal cost, decimal listPrice, int reorderLevel, int safetyStock, bool isActive)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null) throw new NotFoundException($"Product with ID {productId} not found.");

        product.ProductName = productName;
        product.Description = description;
        product.CategoryId = categoryId;
        product.UnitOfMeasure = unitOfMeasure;
        product.Cost = cost;
        product.ListPrice = listPrice;
        product.ReorderLevel = reorderLevel;
        product.SafetyStock = safetyStock;
        product.IsActive = isActive;

        _productRepository.Update(product);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteProductAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null) throw new NotFoundException($"Product with ID {id} not found.");

        // Check if stock exists before deleting
        var stockLevels = await _stockLevelRepository.GetByProductIdAsync(id);
        if (stockLevels.Any(s => s.QuantityOnHand > 0))
        {
            throw new BadRequestException("Cannot delete product with existing stock.");
        }

        _productRepository.Delete(product);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<object> GetProductByIdAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null) throw new NotFoundException($"Product with ID {id} not found.");

        return _mapper.Map<ProductDto>(product);
    }

    public async Task<object> GetAllProductsAsync(int pageNumber, int pageSize)
    {
        var products = await _productRepository.GetPagedAsync(pageNumber, pageSize);
        var totalCount = await _productRepository.GetTotalCountAsync();

        return new PagedResponse<ProductDto>(
            _mapper.Map<IEnumerable<ProductDto>>(products),
            totalCount,
            pageNumber,
            pageSize
        );
    }
}
