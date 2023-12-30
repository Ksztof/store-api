using PerfumeStore.Domain.DTO.Response.Cart;

namespace PerfumeStore.Application.DTOs.Response
{
    public class OrderResponse
    {
        public int Id { get; set; }
        public decimal TotalCartValue { get; set; }
        public IEnumerable<CheckCartDomRes> AboutProductsInCart { get; set; }
        public ShippingDetailResponse ShippingDetil { get; set; }
    }
}