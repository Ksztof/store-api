using PerfumeStore.Core.GenericInterfaces;
using PerfumeStore.Core.RequestForms;
using PerfumeStore.Domain.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Core.Services
{
	public interface IProductsService
    {
        public Task<Product> CreateProductAsync(CreateProductForm createProductForm);
        public Task<Product> UpdateProductAsync(UpdateProductForm updateform);
        public Task DeleteProductAsync(int productId);
        public Task<Product> GetProductByIdAsync(int productId);
        public Task<IEnumerable<Product>> GetAllProductsAsync();
    }
}
