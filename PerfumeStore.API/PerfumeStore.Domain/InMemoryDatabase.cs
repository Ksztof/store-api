using Microsoft.EntityFrameworkCore;
using PerfumeStore.Domain.DbModels;

namespace PerfumeStore.Domain
{
    public class ShopDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> carts { get; set; }
        public DbSet<CartLine> cartsLine { get; set; }
        public DbSet<ProductCategory> productCategories{ get; set; }

        public ShopDbContext(DbContextOptions<ShopDbContext> options) : base(options)
        {
        }

        /* public static List<Product> products = new List<Product>();
         public static List<ProductCategory> productCategories = new List<ProductCategory>();
         public static List<Cart> carts = new List<Cart>();*/

        static ShopDbContext()
        {
            //Add Categories
            var perfumeCategory = new ProductCategory
            {
                Id = 1,
                Name = "Perfume"
            };
            productCategories.Add(perfumeCategory);

            var accessoriesCategory = new ProductCategory
            {
                Id = 2,
                Name = "Accessories"
            };
            productCategories.Add(accessoriesCategory);

            //Add Products
            var perfume1 = new Product
            {
                Id = 1,
                Name = "Perfum1",
                Price = 500,
                Description = "perfum o pięknym zapachu",
                Manufacturer = "Bialy jelen",
                CategoryId = 1,
                DateAdded = DateTime.Now,
            };
            products.Add(perfume1);

            var portfel = new Product
            {
                Id = 2,
                Name = "Portfel",
                Price = 1250,
                Description = "Superancki portfelik",
                Manufacturer = "Luj witom",
                CategoryId = 2,
                DateAdded = DateTime.Now,
            };
            products.Add(portfel);

            var choinkaZapachowa = new Product
            {
                Id = 3,
                Name = "Choinka zapachowa",
                Price = 25,
                Description = "Superancki portfelik",
                Manufacturer = "Choinka samochodowa o zapachu waniliowym",
                CategoryId = 2,
                DateAdded = DateTime.Now,
            };
            products.Add(choinkaZapachowa);
        }
    }
}
