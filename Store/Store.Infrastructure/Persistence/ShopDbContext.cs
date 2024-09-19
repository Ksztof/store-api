using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Store.Domain.CarLines;
using Store.Domain.Carts;
using Store.Domain.Orders;
using Store.Domain.ProductCategories;
using Store.Domain.ProductProductCategories;
using Store.Domain.Products;
using Store.Domain.StoreUsers;

namespace Store.Infrastructure.Persistence;

public class ShopDbContext : IdentityDbContext<StoreUser>
{
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductCategory> ProductCategories { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartLine> CartsLines { get; set; }
    public DbSet<ProductProductCategory> ProductProductCategories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<ShippingDet> ShippingDetails { get; set; }

    public ShopDbContext(DbContextOptions<ShopDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Cart>()
            .HasMany(c => c.CartLines);

        modelBuilder.Entity<CartLine>()
            .HasOne(cl => cl.Product)
            .WithMany()
            .HasForeignKey(cl => cl.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.Cart)
            .WithOne(c => c.Order)
            .HasForeignKey<Order>(o => o.CartId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.StoreUser)
            .WithMany(su => su.Orders)
            .HasForeignKey(o => o.StoreUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ProductProductCategory>()
            .HasKey(ppc => new { ppc.ProductId, ppc.ProductCategoryId });

        modelBuilder.Entity<ProductProductCategory>()
            .HasOne(pc => pc.Product)
            .WithMany(p => p.ProductProductCategories)
            .HasForeignKey(pc => pc.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ProductProductCategory>()
            .HasOne(pc => pc.ProductCategory)
            .WithMany(c => c.ProductProductCategories)
            .HasForeignKey(pc => pc.ProductCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StoreUser>()
            .HasOne(su => su.Cart)
            .WithOne(c => c.StoreUser)
            .HasForeignKey<Cart>(c => c.StoreUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.ShippingDetail)
            .WithOne(sd => sd.Order)
            .HasForeignKey<Order>(o => o.ShippingDetailId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}