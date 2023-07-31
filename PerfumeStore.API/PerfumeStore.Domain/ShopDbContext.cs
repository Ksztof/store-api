using Microsoft.EntityFrameworkCore;
using PerfumeStore.Domain.DbModels;

namespace PerfumeStore.Domain
{
    public class ShopDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartLine> CartsLine { get; set; }
        public DbSet<ProductProductCategory> ProductProductCategories { get; set; }
        public DbSet<Order> Orders { get; set; }


        public ShopDbContext(DbContextOptions<ShopDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cart>()
                .HasMany(c => c.CartLines);

            modelBuilder.Entity<Product>()
                .HasOne<CartLine>(cl => cl.CartLine)
                .WithOne(cl => cl.Product)
                .HasForeignKey<CartLine>(cl => cl.ProductId);

            modelBuilder.Entity<Cart>()
                .HasOne<Order>(o => o.Order)
                .WithOne(o => o.Cart)
                .HasForeignKey<Order>(o => o.CartId);

            modelBuilder.Entity<ProductProductCategory>()
                 .HasKey(pc => new { pc.ProductId, pc.ProductCategoryId });

            modelBuilder.Entity<ProductProductCategory>()
                .HasOne<Product>(pc => pc.Product)
                .WithMany(p => p.ProductProductCategories)
                .HasForeignKey(pc => pc.ProductId);

            modelBuilder.Entity<ProductProductCategory>()
                .HasOne<ProductCategory>(pc => pc.ProductCategory)
                .WithMany(c => c.ProductProductCategories)
                .HasForeignKey(pc => pc.ProductCategoryId);
        }
    }
}
