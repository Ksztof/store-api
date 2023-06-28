using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Domain
{
	public class Product
	{
		[Key]
		public int ProductId { get; set; }
		public string ProductName { get; set; }
		public double? Rating { get; set; }
		public double ProductPrice { get; set; }
		public decimal? Discount { get; set; }
		//public ProductPhotos ProductPhotos {get; set;}
		public string? ProductDescription { get; set; }
		public double ProductStock { get; set; }
		//public ProductCategory ProductCategory { get; set; }
		public string? ProductManufacturer { get; set; }
		public bool IsProductRecommended { get; set; } 
		public bool IsProductActive { get; set; }
		public DateTime DateAdded { get; set; }
		public DateTime? DateModified { get; set; }
		//public Currency currency { get; set; }
	}
}
