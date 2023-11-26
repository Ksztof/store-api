using AutoMapper;
using PerfumeStore.Application.DTOs.Request;
using PerfumeStore.Domain.DTOs.Request;
using PerfumeStore.Domain.StoreUsers;

namespace PerfumeStore.Application.Mapper
{
    public class MappingProfileApplication : Profile
    {
        public MappingProfileApplication()
        {
            CreateMap<UserForRegistrationDto, StoreUser>()
                .ForMember(u => u.UserName, opt => opt.MapFrom(x => x.Email));

            CreateMap<AddProductsToCartDtoApplication, AddProductsToCartDtoDomain>();
            CreateMap<ProductInCartApplication, ProductInCartDomain>();

            CreateMap<ModifyProductDtoApplication, ModifyProductDtoDomain>();
            CreateMap<ProductModificationApplication, ProductModificationDomain>();
        }
    }
}
