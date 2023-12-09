using PerfumeStore.Domain.CarLines;
using PerfumeStore.Domain.Core.DTO;
using PerfumeStore.Domain.Interfaces;
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
        public string? Description { get; set; }
        public string? Manufacturer { get; set; }
        public DateTime DateAdded { get; set; }
        //public ICollection<CartLine> CartLines { get; set; } = new List<CartLine>();
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
            Name = updateProductForm.ProductName; //:TODO check if name has been changed if not, leave old value
            Price = updateProductForm.ProductPrice;
            Description = updateProductForm.ProductDescription;
            ProductProductCategories = productCategories.Select(pc => new ProductProductCategory
            {
                Product = this,
                ProductCategory = pc
            }).ToList();
            Manufacturer = updateProductForm.ProductManufacturer;
            DateAdded = DateTime.Now;
        }
    }
}