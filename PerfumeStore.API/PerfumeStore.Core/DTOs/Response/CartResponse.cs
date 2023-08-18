using System.Text.Json.Serialization;

namespace PerfumeStore.Core.DTOs.Response
{
    public class CartResponse
    {
        public int Id { get; set; }
        public IEnumerable<CartLineResponse> CartLineResponse { get; set; }
        [JsonIgnore]
        public IEnumerable<string>? Errors { get; set; }
    }
}