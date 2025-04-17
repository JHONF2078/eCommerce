using AutoMapper;
using ProductsService.DataAccessLayer.Entities;
using ProductsService.BusinessLogicLayer.DTO;

namespace ProductsService.BusinessLogicLayer.Mappers;

public class ProductUpdateRequestToProductMappingProfile : Profile
{
  public ProductUpdateRequestToProductMappingProfile()
  {
       var map = CreateMap<ProductUpdateRequest, Product>()
       // ──────────────────────────────────────────────
       // Reglas específicas por propiedad
       // ──────────────────────────────────────────────
       .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductName))
       .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
       //.ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
       // ──────────────────────────────────────────────
       // UnitPrice: solo copiar cuando llegue un valor
       //            (double? => HasValue)
       // ──────────────────────────────────────────────
       .ForMember(dest => dest.UnitPrice,
        opt =>
        {
            opt.Condition(src => src.UnitPrice.HasValue);   // ← solo si no es null
            opt.MapFrom(src => src.UnitPrice);
        })
       .ForMember(dest => dest.QuantityInStock, opt => opt.MapFrom(src => src.QuantityInStock))
       .ForMember(dest => dest.ProductID, opt => opt.MapFrom(src => src.ProductID));

        // ────────────────────────────────────────────── // ──────────────────────────────────────────────
        // Regla genérica: copiar solo miembros ≠ null
        // AutoMapper solo actualizará propiedades con valores
        // (útil para PATCH parcial)
        // ──────────────────────────────────────────────
        // map.ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
    }
}
