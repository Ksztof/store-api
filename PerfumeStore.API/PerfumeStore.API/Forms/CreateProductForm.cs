using PerfumeStore.Domain.Models;

namespace PerfumeStore.API.Forms
{
	public class CreateProductForm
	{
		public int ProductId { get; set; }
		public string ProductName { get; set; }
		public double ProductPrice { get; set; }
		public string ProductDescription { get; set; }
		public ProductCategory ProductCategory { get; set; }
		public string? ProductManufacturer { get; set; }
		public DateTime DateAdded { get; set; }
	}
}
