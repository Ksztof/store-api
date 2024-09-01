using AutoMapper;
using Store.Application.Carts.Dto.Request;
using Store.Application.Orders.Dto.Request;
using Store.Application.Orders.Dto.Response;
using Store.Application.Products.Dto.Request;
using Store.Application.Shared.DTO.Response;
using Store.Domain.CarLines;
using Store.Domain.Orders;
using Store.Domain.Shared.DTO.models;
using Store.Domain.Shared.DTO.Request.Order;
using Store.Domain.Shared.DTO.Request.Product;


namespace Store.Application.Shared.Mapper
{
    public class MappingProfileApplication : Profile
    {
        public MappingProfileApplication()
        {
            CreateMap<NewProductsDtoApp, NewProductsDtoDom>();
            CreateMap<ProductInCartApp, ProductInCartDom>();

            CreateMap<ModifyProductDtoApp, ModifyProductDtoDom>();
            CreateMap<ProductModificationApp, ProductModificationDom>();

            CreateMap<CreateProductDtoApp, CreateProductDtoDom>();

            CreateMap<UpdateProductDtoApp, UpdateProductDtoDom>();

            CreateMap<CreateOrderDtoApp, CreateOrderDtoDom>();

            CreateMap<ShippingDet, ShippingDetailResponse>();

            CreateMap<CartLine, CartLineResponse>()
                .ForMember(dest => dest.productId, opt => opt.MapFrom(src => src.Product.Id))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.Product.Price))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Quantity * src.Product.Price));

            CreateMap<ShippingDet, ShippingInfo>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Street))
                .ForMember(dest => dest.StreetNumber, opt => opt.MapFrom(src => src.StreetNumber))
                .ForMember(dest => dest.HomeNumber, opt => opt.MapFrom(src => src.HomeNumber))
                .ForMember(dest => dest.PostCode, opt => opt.MapFrom(src => src.PostCode))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber));
        }
    }
}
