using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PerfumeStore.Domain.CarLines;
using PerfumeStore.Domain.Carts;
using PerfumeStore.Domain.Orders;
using PerfumeStore.Domain.ProductCategories;
using PerfumeStore.Domain.ProductProductCategories;
using PerfumeStore.Domain.Products;
using PerfumeStore.Domain.ShippingDetails;
using PerfumeStore.Domain.StoreUsers;

namespace PerfumeStore.Infrastructure
{
    public class ShopDbContext : IdentityDbContext<StoreUser>
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartLine> CartsLine { get; set; }
        public DbSet<ProductProductCategory> ProductProductCategories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<ShippingDet> ShippingDetails { get; set; }


        public ShopDbContext(DbContextOptions<ShopDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Cart>()
                .HasMany(c => c.CartLines);

            modelBuilder.Entity<CartLine>()
                .HasOne(cl => cl.Product)
                .WithMany()
                .HasForeignKey(cl => cl.ProductId);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Cart)
                .WithMany()
                .HasForeignKey(o => o.CartId);

            modelBuilder.Entity<ProductProductCategory>()
                 .HasKey(pc => new { pc.Id });

            modelBuilder.Entity<ProductProductCategory>()
                .HasOne<Product>(pc => pc.Product)
                .WithMany(p => p.ProductProductCategories)
                .HasForeignKey(pc => pc.ProductId);

            modelBuilder.Entity<ProductProductCategory>()
                .HasOne<ProductCategory>(pc => pc.ProductCategory)
                .WithMany(c => c.ProductProductCategories)
                .HasForeignKey(pc => pc.ProductCategoryId);

            modelBuilder.Entity<StoreUser>()
                .HasOne(su => su.Cart)
                .WithOne(c => c.StoreUser)
                .HasForeignKey<Cart>(c => c.StoreUserId);

            modelBuilder.Entity<Order>()
                .HasOne(o  => o.ShippingDetail)
                .WithOne(sd => sd.Order)
                .HasForeignKey<Order>(o => o.ShippingDetailId);
        }
    }
}