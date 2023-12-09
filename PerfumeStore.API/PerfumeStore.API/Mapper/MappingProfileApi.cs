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
            CreateMap<AddProductsToCartDtoApi, AddProductsToCartDtoApp>();
            CreateMap<ProductInCartApi, ProductInCartApp>();

            CreateMap<ModifyProductDtoApi, ModifyProductDtoApp>();
            CreateMap<ProductModificationApi, ProductModificationApp>();

            CreateMap<AuthenticateUserDtoApi, AuthenticateUserDtoApp>();

            CreateMap<RegisterUserDtoApi, RegisterUserDtoApp>();

            CreateMap<CreateProductDtoApi, CreateProductDtoApp>();

            CreateMap<UpdateProductDtoApi, UpdateProductDtoApp>();

            CreateMap<RegisterUserDtoApi, StoreUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));
        }
    }
}
