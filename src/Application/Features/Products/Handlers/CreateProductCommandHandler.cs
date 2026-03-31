using MediatR;
using AutoMapper;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Interfaces.Repositories;
using InventoryManagement.Interfaces;

namespace InventoryManagement.Application.Features.Products.Handlers;

public class CreateProductCommandHandler : IRequestHandler<Commands.CreateProductCommand, Guid>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateProductCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(Commands.CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = _mapper.Map<Product>(request);
        product.ProductId = Guid.NewGuid();
        product.IsActive = true;

        await _productRepository.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();

        return product.ProductId;
    }
}
