using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PerfumeStore.Domain.Models;

namespace PerfumeStore.Domain
{
    public static class InMemoryDatabase
	{
		public static List<Product> products = new List<Product>();
		public static List<ProductCategories> productCategories = new List<ProductCategories>();

		static InMemoryDatabase()
		{
			//Add Categories
			var perfumeCategory = new ProductCategories
			{
				ProductCategoryId = 1,
				Name = "Perfume"
			};
			productCategories.Add(perfumeCategory);

			var accessoriesCategory = new ProductCategories
			{
				ProductCategoryId = 2,
				Name = "Accessories"
			};
			productCategories.Add(accessoriesCategory);

			//Add Products
			var perfume1 = new Product
			{
				ProductId = 1,
				Name = "Perfum1",
				Price = 500,
				Description = "perfum o pięknym zapachu",
				Manufacturer = "Bialy jelen",
				CategoryId = 1,
				DateAdded = DateTime.Now,
			};
			products.Add(perfume1);

			var portfel = new Product
			{
				ProductId = 2,
				Name = "Portfel",
				Price = 1250,
				Description = "Superancki portfelik",
				Manufacturer = "Luj witom",
				CategoryId = 2,
				DateAdded = DateTime.Now,
			};
			products.Add(portfel);

			var choinkaZapachowa = new Product
			{
				ProductId = 3,
				Name = "Choinka zapachowa",
				Price = 25,
				Description = "Superancki portfelik",
				Manufacturer = "Choinka samochodowa o zapachu waniliowym",
				CategoryId = 2,
				DateAdded = DateTime.Now,
			};
			products.Add(choinkaZapachowa);
		}
	}
}
