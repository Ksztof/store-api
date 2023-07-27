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

            modelBuilder.Entity<ProductProductCategory>()
                 .HasKey(pc => new { pc.ProductId, pc.ProductCategoryId });

            modelBuilder.Entity<ProductProductCategory>()
                .HasOne<Product>(pc => pc.Product)
                .WithMany(p => p.ProductProductCategories)
                .HasForeignKey(pc => pc.ProductId).
                OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<ProductProductCategory>()
                .HasOne<ProductCategory>(pc => pc.ProductCategory)
                .WithMany(c => c.ProductProductCategories)
                .HasForeignKey(pc => pc.ProductCategoryId).
                OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
/*modelBuilder.Entity<Cart>()
                .HasMany(c => c.CartLines)
                .WithOne(e => e.Cart)
                .HasForeignKey(e => e.CartId);

modelBuilder.Entity<Product>()
    .HasOne<CartLine>(cl => cl.CartLine)
    .WithOne(cl => cl.Product)
    .HasForeignKey<CartLine>(cl => cl.ProductId);

modelBuilder.Entity<ProductProductCategory>()
     .HasKey(pc => new { pc.ProductId, pc.ProductCategoryId });

modelBuilder.Entity<ProductProductCategory>()
    .HasOne<Product>(pc => pc.Product)
    .WithMany(p => p.ProductProductCategories)
    .HasForeignKey(pc => pc.ProductId);

modelBuilder.Entity<ProductProductCategory>()
    .HasOne<ProductCategory>(pc => pc.ProductCategory)
    .WithMany(c => c.ProductProductCategories)
    .HasForeignKey(pc => pc.ProductCategoryId);*/