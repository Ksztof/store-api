using PerfumeStore.Core.Forms;
using PerfumeStore.Core.GenericInterfaces;
using PerfumeStore.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Core.Services.ProductsService
{
	public interface IProductsService
	{
		public Task<int> CreateProductAsync(CreateProductForm createProductForm);
		public Task<int> UpdateProductAsync(UpdateProductForm updateform);
		public Task<int> DeleteProductAsync(int productId);
		public Task<Products> GetProductByIdAsync(int productId);
		public Task<IEnumerable<Products>> GetAllProductsAsync();
	}
}
