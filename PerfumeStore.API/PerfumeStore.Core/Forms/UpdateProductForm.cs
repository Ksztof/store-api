namespace PerfumeStore.Core.Forms
{
	public class UpdateProductForm
	{
		public int? ProductId { get; set; }
		public string? ProductName { get; set; }
		public double? ProductPrice { get; set; }
		public string? ProductDescription { get; set; }
		public string? ProductCategory { get; set; } //TODO: jaki typ, czy to powinien być string bo ktos na strone sobie np. z dropdown listy wybierze nową kategorię?
		public string? ProductManufacturer { get; set; } 
	}
}
