using PerfumeStore.Domain.Interfaces;

namespace PerfumeStore.Domain.DbModels
{
    public class ProductCategories : IEntity
    {
        public int ProductCategoryId { get; set; }
        public string Name { get; set; }
    }
}
