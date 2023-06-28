using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Domain
{
	public static class InMemoryDatabase
	{
		public static List<Product> products = new List<Product>();
		//public static IReadOnlyList<Product> Products => products;


		static InMemoryDatabase()
		{
			products.Add(new Product
			{
				ProductId = 1,
				ProductName = "Perfum1",
				Rating = 4.5,
				ProductPrice = 500,
				Discount = 0,
				ProductDescription = "perfum o pięknym zapachu",
				ProductStock = 200,
				ProductManufacturer = "Bialy jelen",
				IsProductRecommended = false,
				IsProductActive = true,
				DateAdded = DateTime.Now,
				DateModified = null
			});

			products.Add(new Product
			{
				ProductId = 2,
				ProductName = "Portfel",
				Rating = 5,
				ProductPrice = 1250,
				Discount = 125,
				ProductDescription = "Superancki portfelik",
				ProductStock = 24,
				ProductManufacturer = "Luj witom",
				IsProductRecommended = true,
				IsProductActive = true,
				DateAdded = DateTime.Now,
				DateModified = null
			});

			products.Add(new Product
			{
				ProductId = 3,
				ProductName = "Choinka zapachowa",
				Rating = 3.5,
				ProductPrice = 25,
				Discount = 0,
				ProductDescription = "Superancki portfelik",
				ProductStock = 1000,
				ProductManufacturer = "Choinka samochodowa o zapachu waniliowym",
				IsProductRecommended = false,
				IsProductActive = false,
				DateAdded = DateTime.Now,
				DateModified = null
			});
		}
	}
}
