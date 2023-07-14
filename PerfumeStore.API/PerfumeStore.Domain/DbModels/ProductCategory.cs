using PerfumeStore.Domain.Interfaces;

namespace PerfumeStore.Domain.DbModels
{
    public class ProductCategories : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
