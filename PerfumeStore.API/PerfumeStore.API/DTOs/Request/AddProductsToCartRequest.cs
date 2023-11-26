using PerfumeStore.API.DTOs.Request;

namespace PerfumeStore.API.DTOs.Request
{
    public class AddProductsToCartRequest
    {
        public ProductInCartRequest[] Products { get; set; }
    }
}