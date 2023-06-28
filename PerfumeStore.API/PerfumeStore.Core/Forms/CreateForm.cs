using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Core.Forms
{
	public class CreateForm
	{
		public string ProductName { get; set; }
		public double ProductPrice { get; set; }
		public decimal? Discount { get; set; }
		public string ProductDescription { get; set; }
		public double ProductStock { get; set; }
		public string? ProductManufacturer { get; set; }
		public bool IsProductRecommended { get; set; }
		public bool IsProductActive { get; set; }
	}
}
