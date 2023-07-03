using Microsoft.AspNetCore.Mvc;
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

		public void CreateAsync(Products item)
		{
			throw new NotImplementedException();
		}

		public void DeleteAsync(int id)
		{
			throw new NotImplementedException();
		}

		public List<Products> GetAllAsync()
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
	}
}
