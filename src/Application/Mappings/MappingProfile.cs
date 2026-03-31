using AutoMapper;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Application.Features.Products.DTOs;
using InventoryManagement.Application.Features.Products.Commands;
using InventoryManagement.Application.Features.Stock.DTOs;

namespace InventoryManagement.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.CategoryName : string.Empty));
        
        CreateMap<CreateProductCommand, Product>();

        CreateMap<StockLevel, StockLevelDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.ProductName : string.Empty))
            .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.Warehouse != null ? src.Warehouse.WarehouseName : string.Empty));
    }
}
