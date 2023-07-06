using Microsoft.AspNetCore.Mvc;
using PerfumeStore.Domain;
using PerfumeStore.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace PerfumeStore.Core.Repositories
{
    public class ProductsRepository : IProductsRepository
	{
		public ProductsRepository()
		{
		}

		public async Task<int> CreateAsync(Products item) 		
		{
			item.ProductCategoryId = GetCurrentProductId();
			InMemoryDatabase.products.Add(item);
			int createdProductId = item.ProductId;
			return await Task.FromResult(createdProductId);
		}

		public async Task<int> DeleteAsync(int id)
		{
			Products productToDelete = await GetByIdAsync(id);
			InMemoryDatabase.products.Remove(productToDelete);

			return await Task.FromResult(productToDelete.ProductId);
		}

		public async Task<IEnumerable<Products>> GetAllAsync()
		{
			IEnumerable<Products> productsList = InMemoryDatabase.products;

			return await Task.FromResult(productsList);
		}

		public async Task<Products> GetByIdAsync(int id)
		{
			Products product = InMemoryDatabase.products.First(x => x.ProductId == id);
			return await Task.FromResult(product);
		}

		public async Task<int> UpdateAsync(Products item)
		{
			Products productById = await GetByIdAsync(item.ProductId); //temporary variable
			int productToUpdateIndex = InMemoryDatabase.products.IndexOf(productById);
			InMemoryDatabase.products[productToUpdateIndex] = item; 

			return await Task.FromResult(item.ProductId);
		}

		private int GetCurrentProductId()
		{
			int lastProductId = InMemoryDatabase.products.Max(x => x.ProductId);
			int currentProductId = lastProductId + 1;
			return currentProductId;
		}
	}
}
