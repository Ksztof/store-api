using PerfumeStore.Domain.Abstractions;
using PerfumeStore.Domain.Products;

namespace PerfumeStore.Domain.CarLines
{
    public class CartLine : Entity
    {
        public CartLine(int productId)
        {
            ProductId = productId;
        }

        private CartLine() { }


        public decimal Quantity { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; }
    }
}