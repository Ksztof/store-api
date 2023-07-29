using System.Text.Json.Serialization;

namespace PerfumeStore.Core.DTOs.Response
{
    public class ProductResponse
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string? Manufacturer { get; set; }
        public DateTime DateAdded { get; set; }
    }
}
