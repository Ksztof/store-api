using PerfumeStore.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Domain.Models
{
    public class Products : IEntity
    {
        [Key]
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public double ProductPrice { get; set; }
		public string ProductDescription { get; set; }
		public ProductCategories ProductCategory { get; set; }
        public string? ProductManufacturer { get; set; }
        public DateTime DateAdded { get; set; }
	}
}
