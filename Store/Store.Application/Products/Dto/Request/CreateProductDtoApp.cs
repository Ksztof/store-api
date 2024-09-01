namespace Store.Application.Products.Dto.Request
{
    public class CreateProductDtoApp
    {
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public string ProductDescription { get; set; }
        public ICollection<int> ProductCategoriesIds { get; set; }
        public string? ProductManufacturer { get; set; }
    }
}
