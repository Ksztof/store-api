using System.Text.Json.Serialization;

namespace PerfumeStore.Application.DTOs.Response
{
    public class CartResponse
    {
        public int? Id { get; set; }
        public IEnumerable<CartLineResponse> CartLineResponse { get; set; }
    }
}