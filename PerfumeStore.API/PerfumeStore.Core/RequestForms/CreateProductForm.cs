using PerfumeStore.Domain.Models;

namespace PerfumeStore.Core.RequestForms
{
	public class CreateProductForm
	{
		public string ProductName { get; set; }
		public decimal ProductPrice { get; set; }
		public string ProductDescription { get; set; }
		public int ProductCategoryId { get; set; }
		public string? ProductManufacturer { get; set; }
		public DateTime DateAdded { get; set; }
	}
}
