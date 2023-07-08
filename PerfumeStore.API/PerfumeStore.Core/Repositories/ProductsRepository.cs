using Microsoft.AspNetCore.Mvc;
using PerfumeStore.Domain;
using PerfumeStore.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
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

        public async Task<int> CreateAsync(Product item)
        {
            item.ProductId = IdGenerator.GetNextId();
            InMemoryDatabase.products.Add(item);
            int createdProductId = item.ProductId;
            return await Task.FromResult(createdProductId);
        }

        public async Task DeleteAsync(int id)
        {
            Product productToDelete = await GetByIdAsync(id);
            InMemoryDatabase.products.Remove(productToDelete);
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            IEnumerable<Product> productsList = InMemoryDatabase.products;

            return await Task.FromResult(productsList);
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            Product product = InMemoryDatabase.products.FirstOrDefault(x => x.ProductId == id);
            return await Task.FromResult(product);
        }

        public async Task<Product> UpdateAsync(Product item)
        {
            Product productById = await GetByIdAsync(item.ProductId); //temporary variable
            int productToUpdateIndex = InMemoryDatabase.products.IndexOf(productById);
            InMemoryDatabase.products[productToUpdateIndex] = item;

            return await Task.FromResult(item);
        }

        private int GetCurrentProductId()
        {
            int lastProductId = InMemoryDatabase.products.Max(x => x.ProductId);
            int currentProductId = lastProductId + 1;
            return currentProductId;
        }
    }
}
