using PerfumeStore.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace PerfumeStore.Domain.DbModels
{
  public class ProductCategory : IEntity<int>
  {
    [Key]
    public int Id { get; set; }

    public string Name { get; set; }
    public ICollection<ProductProductCategory> ProductProductCategories { get; set; }
  }
}