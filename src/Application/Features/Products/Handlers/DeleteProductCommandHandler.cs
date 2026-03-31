using MediatR;
using InventoryManagement.Application.Features.Products.Commands;
using InventoryManagement.Interfaces.Repositories;
using InventoryManagement.Shared.Exceptions;
using InventoryManagement.Interfaces;

namespace InventoryManagement.Application.Features.Products.Handlers;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Unit>
{
    private readonly IProductRepository _productRepository;
    private readonly IStockLevelRepository _stockLevelRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProductCommandHandler(IProductRepository productRepository, IStockLevelRepository stockLevelRepository, IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _stockLevelRepository = stockLevelRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId);
        if (product == null)
        {
            throw new NotFoundException($"Product with ID {request.ProductId} not found.");
        }

        var stocks = await _stockLevelRepository.GetByProductIdAsync(request.ProductId);
        if (stocks.Any(s => s.QuantityOnHand > 0))
        {
            throw new ConflictException("Cannot delete product because it has active stock.");
        }

        _productRepository.Delete(product);
        await _unitOfWork.SaveChangesAsync();

        return Unit.Value;
    }
}
