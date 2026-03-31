using MediatR;
using InventoryManagement.Application.Features.Products.DTOs;
using AutoMapper;
using InventoryManagement.Interfaces.Repositories;
using InventoryManagement.Shared.Exceptions;

namespace InventoryManagement.Application.Features.Products.Handlers;

public class GetProductByIdQueryHandler : IRequestHandler<Queries.GetProductByIdQuery, ProductDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetProductByIdQueryHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<ProductDto> Handle(Queries.GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId);
        if (product == null)
        {
            throw new NotFoundException($"Product with ID {request.ProductId} not found.");
        }

        return _mapper.Map<ProductDto>(product);
    }
}
