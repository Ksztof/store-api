namespace PerfumeStore.Domain.Models
{
    public class CreateProductForm
    {
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public string ProductDescription { get; set; }
        public ICollection<int> ProductCategoriesIds { get; set; }
        public string? ProductManufacturer { get; set; }
        public DateTime DateAdded { get; set; }
    }
}
