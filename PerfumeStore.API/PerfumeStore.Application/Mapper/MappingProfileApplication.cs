using AutoMapper;
using PerfumeStore.Application.DTOs.Request;
using PerfumeStore.Domain.Core.DTO;
using PerfumeStore.Domain.DTOs.Request;

namespace PerfumeStore.Application.Mapper
{
    public class MappingProfileApplication : Profile
    {
        public MappingProfileApplication()
        {
            CreateMap<AddProductsToCartDtoApp, AddProductsToCartDtoDom>();
            CreateMap<ProductInCartApp, ProductInCartDom>();

            CreateMap<ModifyProductDtoApp, ModifyProductDtoDom>();
            CreateMap<ProductModificationApp, ProductModificationDom>();

            CreateMap<CreateProductDtoApp, CreateProductDtoDom>();

            CreateMap<UpdateProductDtoApp, UpdateProductDtoDom>();

            CreateMap<CreateOrderDtoApp, CreateOrderDtoDom>();
        }
    }
}
