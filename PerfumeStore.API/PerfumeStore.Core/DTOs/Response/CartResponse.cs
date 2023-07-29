using System.Text.Json.Serialization;

namespace PerfumeStore.Core.DTOs.Response
{
    public class CartResponse
    {
        [JsonIgnore]
        public int Id { get; set; }
        public IEnumerable<CartLineResponse> CartLineDto { get; set; }
    }
}
