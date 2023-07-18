using Microsoft.EntityFrameworkCore;
using PerfumeStore.Domain.DbModels;

namespace PerfumeStore.Domain
{
    public class ShopDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ProductProductCategory> ProductProductCategories { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartLine> CartsLine { get; set; }


        public ShopDbContext(DbContextOptions<ShopDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
            .HasMany(p => p.ProductCategories)
            .WithOne(pc => pc.Product)
            .HasForeignKey(cl => cl.ProductId);

            modelBuilder.Entity<Cart>()
            .HasMany(c => c.CartLines)
            .WithOne(cl => cl.Cart)
            .HasForeignKey(cl => cl.CartId);

            modelBuilder.Entity<Product>()
            .HasOne<CartLine>(cl => cl.CartLine)
            .WithOne(cl => cl.Product)
            .HasForeignKey<CartLine>(cl => cl.ProductId);
        }
    }
}
