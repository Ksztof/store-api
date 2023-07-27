using PerfumeStore.Domain.Interfaces;
using PerfumeStore.Domain.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PerfumeStore.Domain.DbModels
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
        public ICollection<ProductCategory> ProductCategories { get; set; }
        [JsonIgnore]
        public CartLine CartLine { get; set; }
        public ICollection<ProductProductCategory> ProductProductCategories { get; set; }


        public void CreateProduct(CreateProductForm createProductForm, ICollection<ProductCategory> productCategories)
        {
            Name = createProductForm.ProductName;
            Price = createProductForm.ProductPrice;
            Description = createProductForm.ProductDescription;
            ProductProductCategories = ProductCategories.Select(pc => new ProductProductCategory
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
            ProductCategories = productCategories;
            Manufacturer = updateForm.ProductManufacturer;
            DateAdded = DateTime.Now;
        }
    }
}
