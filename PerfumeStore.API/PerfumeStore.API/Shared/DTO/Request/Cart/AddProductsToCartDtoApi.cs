using PerfumeStore.API.Shared.DTO.Models;

namespace PerfumeStore.API.Shared.DTO.Request.Cart
{
    public class AddProductsToCartDtoApi
    {
        public ProductInCartApi[] Products { get; set; }
    }
}