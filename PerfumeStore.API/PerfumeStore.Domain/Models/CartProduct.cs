using PerfumeStore.Domain.DbModels;

namespace PerfumeStore.Domain.Models
{
    public class CartProduct
    {
        public Product Product { get; set; }
        public decimal ProductQuantity { get; set; }
    }
}
