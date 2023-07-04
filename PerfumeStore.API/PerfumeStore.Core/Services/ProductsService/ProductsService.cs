using PerfumeStore.Core.Forms;
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
		private readonly IProductCategoriesRepository _productCategoriesRepository;

		public ProductsService(IProductsRepository productsRepository, IProductCategoriesRepository productCategoriesRepository)
		{
			_productsRepository = productsRepository;
			_productCategoriesRepository = productCategoriesRepository;
		}

		public int CreateProductAsync(CreateProductForm createProductForm)
		{
			var productToCreate = new Products
			{
				ProductName = createProductForm.ProductName,
				ProductPrice = createProductForm.ProductPrice,
				ProductDescription = createProductForm.ProductDescription,
				ProductCategoryId = createProductForm.ProductCategoryId,
				ProductManufacturer = createProductForm.ProductManufacturer,
				DateAdded = DateTime.Now
			};

			int createdProductId = _productsRepository.CreateAsync(productToCreate);
			return createdProductId;
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
