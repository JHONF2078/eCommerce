using AutoMapper;
using ProductsService.BusinessLogicLayer.DTO;
using ProductsService.DataAccessLayer.Entities;

namespace ProductsService.BusinessLogicLayer.Mappers
{
    public class ProductToProductDtoMappingProfile : Profile
    {
        public ProductToProductDtoMappingProfile()
        {
            CreateMap<Product, ProductDTO>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductName))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
                .ForMember(dest => dest.QuantityInStock, opt => opt.MapFrom(src => src.QuantityInStock))
                .ForMember(dest => dest.ProductID, opt => opt.MapFrom(src => src.Id));
        }
    }
 }
