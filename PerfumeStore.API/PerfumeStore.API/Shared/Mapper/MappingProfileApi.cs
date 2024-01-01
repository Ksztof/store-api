using AutoMapper;
using PerfumeStore.API.Shared.DTO.Models;
using PerfumeStore.API.Shared.DTO.Request.Cart;
using PerfumeStore.API.Shared.DTO.Request.Order;
using PerfumeStore.API.Shared.DTO.Request.Product;
using PerfumeStore.API.Shared.DTO.Request.StoreUser;
using PerfumeStore.Application.Shared.DTO.Request;
using PerfumeStore.Domain.Entities.StoreUsers;

namespace PerfumeStore.API.Shared.Mapper
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
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Login));

            CreateMap<CrateOrderDtoApi, CreateOrderDtoApp>();
        }
    }
}
