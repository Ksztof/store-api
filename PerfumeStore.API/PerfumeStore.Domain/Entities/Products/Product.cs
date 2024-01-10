﻿using PerfumeStore.Domain.DTO.Request.Product;
using PerfumeStore.Domain.Entities.ProductCategories;
using PerfumeStore.Domain.Entities.ProductProductCategories;
using PerfumeStore.Domain.Repositories.Generics;
using System.ComponentModel.DataAnnotations;

namespace PerfumeStore.Domain.Entities.Products
{
    public class Product : IEntity<int>
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public string? Manufacturer { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime? DateUpdated { get; set; }

        public ICollection<ProductProductCategory> ProductProductCategories { get; set; } = new List<ProductProductCategory>();

        public void CreateProduct(CreateProductDtoDom createProductForm, ICollection<ProductCategory> productCategories)
        {
            Name = createProductForm.ProductName;
            Price = createProductForm.ProductPrice;
            Description = createProductForm.ProductDescription;
            ProductProductCategories = productCategories.Select(pc => new ProductProductCategory
            {
                Product = this,
                ProductCategory = pc
            }).ToList();
            Manufacturer = createProductForm.ProductManufacturer;
            DateAdded = DateTime.Now;
        }

        public void UpdateProduct(UpdateProductDtoDom updateProductForm, ICollection<ProductCategory> productCategories)
        {
            if (updateProductForm.ProductName != null && !string.IsNullOrEmpty(updateProductForm.ProductName))
            {
                Name = updateProductForm.ProductName;
            }

            if (updateProductForm.ProductPrice != null && updateProductForm.ProductPrice != 0.0M)
            {
                Price = updateProductForm.ProductPrice;
            }

            if (updateProductForm.ProductDescription != null && !string.IsNullOrEmpty(updateProductForm.ProductDescription))
            {
                Description = updateProductForm.ProductDescription;
            }

            if (updateProductForm.ProductManufacturer != null && !string.IsNullOrEmpty(updateProductForm.ProductManufacturer))
            {
                Manufacturer = updateProductForm.ProductManufacturer;
            }

            if (productCategories != null && productCategories.Any())
            {
                ProductProductCategories = productCategories.Select(pc => new ProductProductCategory
                {
                    Product = this,
                    ProductCategory = pc
                }).ToList();
            }

            DateUpdated = DateTime.Now;
        }
    }
}