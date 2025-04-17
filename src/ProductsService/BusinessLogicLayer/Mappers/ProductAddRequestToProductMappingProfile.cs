using AutoMapper;
using ProductsService.DataAccessLayer.Entities;
using ProductsService.BusinessLogicLayer.DTO;

namespace ProductsService.BusinessLogicLayer.Mappers;

public class ProductAddRequestToProductMappingProfile : Profile
{
  public ProductAddRequestToProductMappingProfile()
  {
       // cuando se crea un nuevo producto a partir del DTO, AutoMapper sabra como construir un objeto Product
       CreateMap<ProductAddRequest, Product>()
      .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductName))
      .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
      .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
      .ForMember(dest => dest.QuantityInStock, opt => opt.MapFrom(src => src.QuantityInStock))
      //ProductID es la clave primaria que genera automaticamente la base de datos.
      //Al ignorarla, evitas que AutoMapper intente sobrescribirla con un valor nulo o
      //por defecto (lo cual podria causar errores)
      .ForMember(dest => dest.ProductID, opt => opt.Ignore())
      ;
  }
}