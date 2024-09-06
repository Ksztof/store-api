using AutoMapper;
using Store.API.Shared.DTO.Models;
using Store.API.Shared.DTO.Request.Cart;
using Store.API.Shared.DTO.Request.Order;
using Store.API.Shared.DTO.Request.Payments;
using Store.API.Shared.DTO.Request.Product;
using Store.API.Shared.DTO.Request.StoreUser;
using Store.Application.Carts.Dto.Request;
using Store.Application.Orders.Dto.Request;
using Store.Application.Payments.Dto.Request;
using Store.Application.Products.Dto.Request;
using Store.Application.Users.Dto.Request;
using Store.Domain.StoreUsers;

namespace Store.API.Shared.Mapper;

internal class MappingProfileApi : Profile
{
    internal MappingProfileApi()
    {
        CreateMap<NewProductsDtoApi, NewProductsDtoApp>();
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

        CreateMap<CheckCurrentCartDtoApi, CheckCurrentCartDtoApp>();

        CreateMap<GetClientSecretDtoApi, GetClientSecretDtoApp>();

        CreateMap<ConfirmPaymentDtoApi, ConfirmPaymentDtoApp>();

        CreateMap<UpdatePaymentIntentDtoApi, UpdatePaymentIntentDtoApp>();

    }
}
