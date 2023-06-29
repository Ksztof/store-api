using PerfumeStore.Core.Repositories;
using PerfumeStore.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Core.Services.ProductsService
{
	public class ProductsService : IProductsService
	{
		private readonly IProductsRepository _productsRepository;
		public ProductsService(IProductsRepository productsRepository)
		{
			_productsRepository = productsRepository;
		}

		public Task<int> CreateProductAsync()
		{
			throw new NotImplementedException();
		}

		public Task<int> DeleteProductAsync()
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<Products>> GetAllProductsAsync()
		{
			throw new NotImplementedException();
		}

		public Task<Products> GetProductByIdAsync(int productId)
		{
			throw new NotImplementedException();
		}

		public Task<int> UpdateProductAsync()
		{
			throw new NotImplementedException();
		}
	}
}
