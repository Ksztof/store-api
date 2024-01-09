namespace PerfumeStore.API.Shared.DTO.Request.Product
{
    public class UpdateProductDtoApi
    {
        public int productId { get; set; }
        public string? ProductName { get; set; }
        public decimal? ProductPrice { get; set; }
        public string? ProductDescription { get; set; }
        public string? ProductManufacturer { get; set; }
        public ICollection<int>? ProductCategoriesIds { get; set; }
    }
}
