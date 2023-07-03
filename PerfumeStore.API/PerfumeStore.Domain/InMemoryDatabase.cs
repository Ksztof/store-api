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
		public static List<Products> products = new List<Products>();
		public static List<ProductCategory> productCategories = new List<ProductCategory>();

		static InMemoryDatabase()
		{
			//Add Categories
			var perfumeCategory = new ProductCategory
			{
				ProductCategoryId = 1,
				CategoryName = "Perfume"
			};
			productCategories.Add(perfumeCategory);

			var accessoriesCategory = new ProductCategory
			{
				ProductCategoryId = 2,
				CategoryName = "Accessories"
			};
			productCategories.Add(accessoriesCategory);

			//Add Products
			var perfume1 = new Products
			{
				ProductId = 1,
				ProductName = "Perfum1",
				ProductPrice = 500,
				ProductDescription = "perfum o pięknym zapachu",
				ProductManufacturer = "Bialy jelen",
				ProductCategory = perfumeCategory,
				DateAdded = DateTime.Now,
			};
			products.Add(perfume1);

			var portfel = new Products
			{
				ProductId = 2,
				ProductName = "Portfel",
				ProductPrice = 1250,
				ProductDescription = "Superancki portfelik",
				ProductManufacturer = "Luj witom",
				ProductCategory = accessoriesCategory,
				DateAdded = DateTime.Now,
			};
			products.Add(portfel);

			var choinkaZapachowa = new Products
			{
				ProductId = 3,
				ProductName = "Choinka zapachowa",
				ProductPrice = 25,
				ProductDescription = "Superancki portfelik",
				ProductManufacturer = "Choinka samochodowa o zapachu waniliowym",
				ProductCategory = accessoriesCategory,
				DateAdded = DateTime.Now,
			};
			products.Add(choinkaZapachowa);
		}
	}
}
