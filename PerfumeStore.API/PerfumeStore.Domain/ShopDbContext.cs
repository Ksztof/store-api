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
            modelBuilder.Entity<ProductProductCategory>()
              .HasKey(ppc => new { ppc.ProductId, ppc.ProductCategoryId });

            modelBuilder.Entity<ProductProductCategory>()
              .HasOne(ppc => ppc.Product)
              .WithMany(p => p.ProductProductCategories)
              .HasForeignKey(ppc => ppc.ProductId);

            modelBuilder.Entity<ProductProductCategory>()
              .HasOne(ppc => ppc.ProductCategory)
              .WithMany(c => c.ProductProductCategories)
              .HasForeignKey(ppc => ppc.ProductCategoryId);

            modelBuilder.Entity<Cart>()
             .HasMany(c => c.CartLines)
             .WithOne(cl => cl.Cart)
             .HasForeignKey(cl => cl.CartId);

            modelBuilder.Entity<CartLine>()
             .HasOne(cl => cl.Product)
             .WithMany(p => p.CartLines)
             .HasForeignKey(cl => cl.ProductId);

            modelBuilder.Entity<Product>()
             .Property(b => b.Price)
             .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<CartLine>()
             .Property(b => b.Quantity)
             .HasColumnType("decimal(18, 2)");
        }


    }
}
