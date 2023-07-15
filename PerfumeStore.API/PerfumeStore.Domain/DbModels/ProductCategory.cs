using PerfumeStore.Domain.Interfaces;

namespace PerfumeStore.Domain.DbModels
{
    public class ProductCategory : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<ProductProductCategory> ProductProductCategories { get; set; }
    }
}
