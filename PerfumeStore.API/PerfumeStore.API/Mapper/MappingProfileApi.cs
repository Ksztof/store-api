using AutoMapper;
using PerfumeStore.API.DTOs.Request;
using PerfumeStore.Application.DTOs.Request;
using PerfumeStore.Domain.StoreUsers;

namespace PerfumeStore.API.Mapper
{
    public class MappingProfileApi : Profile
    {
        public MappingProfileApi()
        {
            CreateMap<AddProductsToCartRequest, AddProductsToCartDtoApplication>();
            CreateMap<ProductInCartRequest, ProductInCartApplication>();

            CreateMap<ModifyProductRequest, ModifyProductDtoApplication>();
            CreateMap<ProductModificationRequest, ProductModificationApplication>();
        }
    }
}
