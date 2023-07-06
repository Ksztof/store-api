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

		public async Task<int> CreateProductAsync(CreateProductForm createProductForm)
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

			int createdProductId = await _productsRepository.CreateAsync(productToCreate);
			return createdProductId;
		}

		public async Task<int> DeleteProductAsync(int productId)
		{
			int deletedProductId = await _productsRepository.DeleteAsync(productId);
			return await Task.FromResult(deletedProductId);
		}

		public async Task<IEnumerable<Products>> GetAllProductsAsync()
		{
			throw new NotImplementedException();
		}

		public async Task<Products> GetProductByIdAsync(int productId)
		{
			Products product = await _productsRepository.GetByIdAsync(productId);
			return await Task.FromResult(product);
		}

		public async Task<int> UpdateProductAsync(UpdateProductForm updateform)
		{
			Products productToUpdate = await _productsRepository.GetByIdAsync(updateform.ProductId); //TODO:will have to think about smarter mapping/mapper
			productToUpdate.ProductName = updateform.ProductName;
			productToUpdate.ProductPrice = updateform.ProductPrice;
			productToUpdate.ProductDescription = updateform.ProductDescription;
			productToUpdate.ProductManufacturer = updateform.ProductManufacturer;

			int updatedProductId = await _productsRepository.UpdateAsync(productToUpdate);
			return await Task.FromResult(updatedProductId);
		}
	}
}
