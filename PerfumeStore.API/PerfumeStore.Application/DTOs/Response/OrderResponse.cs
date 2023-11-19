using PerfumeStore.Domain.Core.DTO;

namespace PerfumeStore.Application.DTOs.Response
{
    public class OrderResponse
    {
        public int Id { get; set; }
        public decimal TotalCartValue { get; set; }
        public IEnumerable<CheckCartDto> AboutProductsInCart { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}