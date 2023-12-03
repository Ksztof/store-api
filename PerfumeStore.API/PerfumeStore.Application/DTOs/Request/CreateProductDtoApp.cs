using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Application.DTOs.Request
{
    public class CreateProductDtoApp
    {
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public string ProductDescription { get; set; }
        public ICollection<int> ProductCategoriesIds { get; set; }
        public string? ProductManufacturer { get; set; }
        public DateTime DateAdded { get; set; }
    }
}
