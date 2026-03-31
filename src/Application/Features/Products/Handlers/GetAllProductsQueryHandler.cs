using MediatR;
using InventoryManagement.Application.Features.Products.DTOs;
using AutoMapper;
using InventoryManagement.Interfaces.Repositories;

namespace InventoryManagement.Application.Features.Products.Handlers;

public class GetAllProductsQueryHandler : IRequestHandler<Queries.GetAllProductsQuery, Queries.PagedResponse<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetAllProductsQueryHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<Queries.PagedResponse<ProductDto>> Handle(Queries.GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetPagedAsync(request.PageNumber, request.PageSize);
        var totalCount = await _productRepository.GetTotalCountAsync();

        var dtos = _mapper.Map<IEnumerable<ProductDto>>(products);

        return new Queries.PagedResponse<ProductDto>(dtos, totalCount, request.PageNumber, request.PageSize);
    }
}
