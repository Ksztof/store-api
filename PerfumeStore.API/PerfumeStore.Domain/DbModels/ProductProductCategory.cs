namespace PerfumeStore.Domain.DbModels
{
  public class ProductProductCategory
  {
    public int ProductId { get; set; }
    public Product Product { get; set; }

    public int ProductCategoryId { get; set; }
    public ProductCategory ProductCategory { get; set; }
  }
}