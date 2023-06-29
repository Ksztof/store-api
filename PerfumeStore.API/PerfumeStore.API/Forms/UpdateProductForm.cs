namespace PerfumeStore.API.Forms
{
	public class UpdateProductForm
	{
		public int? ProductToUpdateId { get; set; }
		public string? UpdatedProductName { get; set; }
		public double? UpdatedProductPrice { get; set; }
		public string? UpdatedProductDescription { get; set; }
		public string? UpdatedProductCategory { get; set; } //TODO: jaki typ, czy to powinien być string bo ktos na strone sobie np. z dropdown listy wybierze nową kategorię?
	}
}
