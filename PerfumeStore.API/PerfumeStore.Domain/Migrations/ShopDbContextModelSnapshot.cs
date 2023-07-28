﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PerfumeStore.Domain;

#nullable disable

namespace PerfumeStore.Domain.Migrations
{
    [DbContext(typeof(ShopDbContext))]
    partial class ShopDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("PerfumeStore.Domain.DbModels.Cart", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.HasKey("Id");

                    b.ToTable("Carts");
                });

            modelBuilder.Entity("PerfumeStore.Domain.DbModels.CartLine", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("CartId")
                        .HasColumnType("int");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<decimal>("Quantity")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.HasIndex("CartId");

                    b.HasIndex("ProductId")
                        .IsUnique();

                    b.ToTable("CartsLine");
                });

            modelBuilder.Entity("PerfumeStore.Domain.DbModels.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("DateAdded")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Manufacturer")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("PerfumeStore.Domain.DbModels.ProductCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ProductCategories");
                });

            modelBuilder.Entity("PerfumeStore.Domain.DbModels.ProductProductCategory", b =>
                {
                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("ProductCategoryId")
                        .HasColumnType("int");

                    b.HasKey("ProductId", "ProductCategoryId");

                    b.HasIndex("ProductCategoryId");

                    b.ToTable("ProductProductCategories");
                });

            modelBuilder.Entity("PerfumeStore.Domain.DbModels.CartLine", b =>
                {
                    b.HasOne("PerfumeStore.Domain.DbModels.Cart", null)
                        .WithMany("CartLines")
                        .HasForeignKey("CartId");

                    b.HasOne("PerfumeStore.Domain.DbModels.Product", "Product")
                        .WithOne("CartLine")
                        .HasForeignKey("PerfumeStore.Domain.DbModels.CartLine", "ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("PerfumeStore.Domain.DbModels.ProductProductCategory", b =>
                {
                    b.HasOne("PerfumeStore.Domain.DbModels.ProductCategory", "ProductCategory")
                        .WithMany("ProductProductCategories")
                        .HasForeignKey("ProductCategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PerfumeStore.Domain.DbModels.Product", "Product")
                        .WithMany("ProductProductCategories")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");

                    b.Navigation("ProductCategory");
                });

            modelBuilder.Entity("PerfumeStore.Domain.DbModels.Cart", b =>
                {
                    b.Navigation("CartLines");
                });

            modelBuilder.Entity("PerfumeStore.Domain.DbModels.Product", b =>
                {
                    b.Navigation("CartLine")
                        .IsRequired();

                    b.Navigation("ProductProductCategories");
                });

            modelBuilder.Entity("PerfumeStore.Domain.DbModels.ProductCategory", b =>
                {
                    b.Navigation("ProductProductCategories");
                });
#pragma warning restore 612, 618
        }
    }
}
