using PerfumeStore.Domain.CarLines;
using PerfumeStore.Domain.Interfaces;
using PerfumeStore.Domain.Models;
using PerfumeStore.Domain.ProductCategories;
using PerfumeStore.Domain.ProductProductCategories;
using System.ComponentModel.DataAnnotations;

namespace PerfumeStore.Domain.Products
{
    public class Product : IEntity<int>
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string? Manufacturer { get; set; }
        public DateTime DateAdded { get; set; }
        public ICollection<CartLine> CartLines { get; set; }
        public ICollection<ProductProductCategory> ProductProductCategories { get; set; }

        public void CreateProduct(CreateProductForm createProductForm, ICollection<ProductCategory> productCategories)
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

        public void UpdateProduct(UpdateProductForm updateForm, ICollection<ProductCategory> productCategories)
        {
            Name = updateForm.ProductName;
            Price = updateForm.ProductPrice;
            Description = updateForm.ProductDescription;
            ProductProductCategories = productCategories.Select(pc => new ProductProductCategory
            {
                Product = this,
                ProductCategory = pc
            }).ToList();
            Manufacturer = updateForm.ProductManufacturer;
            DateAdded = DateTime.Now;
        }
    }
}