using PerfumeStore.Core.GenericInterfaces;
using PerfumeStore.Domain;
using PerfumeStore.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Core.Repositories.ProductCategories
{
    public class ProductCategoriesRepository : IProductCategoriesRepository
    {
        public Task<int> CreateAsync(ProductCategories item)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ProductCategories>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ProductCategories> GetById(int id)
        {
            ProductCategories productCategory = InMemoryDatabase.productCategories.First(x => x.ProductCategoryId == id);
            return Task.FromResult(productCategory);
        }

        public async Task<ProductCategories> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(ProductCategories item)
        {
            throw new NotImplementedException();
        }
    }
}
