using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Application.DTOs.Request
{
    public class UpdateProductDtoApp //:TODO public? internal class? to check in all models security lvl
    {
        public int productId { get; set; }
        public string? ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public string? ProductDescription { get; set; }
        public string? ProductManufacturer { get; set; }
        public ICollection<int>? ProductCategoriesIds { get; set; }
    }
}
