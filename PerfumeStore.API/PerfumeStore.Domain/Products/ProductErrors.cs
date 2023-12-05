using PerfumeStore.Domain.Abstractions;

namespace PerfumeStore.Domain.Products
{
    public static class ProductErrors
    {
        public static readonly Error Something1 = new("Products.Something1", "Something happend 1");

        public static readonly Error Something2 = new("Products.Something2", "Something happend 2");

        public static readonly Error Something3 = new("Products.Something3", "Something happend 3");
    }
}
