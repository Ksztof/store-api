namespace PerfumeStore.Domain.Models
{
  public class UpdateProductForm
  {
    public string ProductName { get; set; }
    public decimal ProductPrice { get; set; }
    public string ProductDescription { get; set; }
    public string ProductManufacturer { get; set; }
    public ICollection<int> ProductCategoriesIds { get; set; }
  }
}