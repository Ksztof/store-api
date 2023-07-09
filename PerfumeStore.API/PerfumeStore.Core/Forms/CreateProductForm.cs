﻿using PerfumeStore.Domain.Models;

namespace PerfumeStore.Core.Forms
{
	public class CreateProductForm
	{
		public int ProductId { get; set; }
		public string ProductName { get; set; }
		public double ProductPrice { get; set; }
		public string ProductDescription { get; set; }
		public int ProductCategoryId { get; set; }
		public string? ProductManufacturer { get; set; }
		public DateTime DateAdded { get; set; }
	}
}