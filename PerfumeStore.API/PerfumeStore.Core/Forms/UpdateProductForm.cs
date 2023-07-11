using PerfumeStore.Domain.Models;

namespace PerfumeStore.Core.Forms
{
	public class UpdateProductForm
	{
		public int ProductId { get; set; }
		public string ProductName { get; set; }
		public double ProductPrice { get; set; }
		public string ProductDescription { get; set; }
		/*public int CategoryId { get; set; }
		public ProductCategories ProductCategories { get; set; }*/
		public string ProductManufacturer { get; set; } 
	}
}
