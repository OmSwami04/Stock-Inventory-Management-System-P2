using MediatR;
using InventoryManagement.Application.Features.Products.Commands;
using InventoryManagement.Interfaces.Repositories;
using InventoryManagement.Shared.Exceptions;
using InventoryManagement.Interfaces;

namespace InventoryManagement.Application.Features.Products.Handlers;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Unit>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId);
        if (product == null)
        {
            throw new NotFoundException($"Product with ID {request.ProductId} not found.");
        }

        product.ProductName = request.ProductName;
        product.Description = request.Description;
        product.CategoryId = request.CategoryId;
        product.UnitOfMeasure = request.UnitOfMeasure;
        product.Cost = request.Cost;
        product.ListPrice = request.ListPrice;
        product.ReorderLevel = request.ReorderLevel;
        product.SafetyStock = request.SafetyStock;
        product.IsActive = request.IsActive;

        _productRepository.Update(product);
        await _unitOfWork.SaveChangesAsync();

        return Unit.Value;
    }
}
