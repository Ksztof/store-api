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

		public int CreateAsync(Products item) 		
		{
			InMemoryDatabase.products.Add(item);
			int createdProductId = item.ProductId;
			return createdProductId;
		}

		public void DeleteAsync(int id)
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<Products>> GetAllAsync()
		{
			throw new NotImplementedException();
		}

		public Products GetByIdAsync(int id)
		{
			throw new NotImplementedException();
		}

		public void UpdateAsync(Products item)
		{
			throw new NotImplementedException();
		}

		public int GetCurrentProductId()
		{
			int lastProductId = InMemoryDatabase.products.Max(x => x.ProductId);
			int currentProductId = lastProductId + 1;
			return currentProductId;
		}
	}
}
