using PerfumeStore.Domain;
using PerfumeStore.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Core.Repositories
{
	public class ProductCategoriesRepository : IProductCategoriesRepository
	{
		public Task<int> CreateAsync(ProductCategories item)
		{
			throw new NotImplementedException();
		}

		public void DeleteAsync(int id)
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<ProductCategories>> GetAllAsync()
		{
			throw new NotImplementedException();
		}

		public ProductCategories GetById(int id)
		{
			ProductCategories productCategory = InMemoryDatabase.productCategories.First(x => x.ProductCategoryId == id);
			return productCategory;
		}

		public async Task<ProductCategories> GetByIdAsync(int id)
		{
			throw new NotImplementedException();
		}

		public void UpdateAsync(ProductCategories item)
		{
			throw new NotImplementedException();
		}
	}
}
