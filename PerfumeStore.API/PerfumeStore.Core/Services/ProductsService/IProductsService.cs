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
		public int CreateProductAsync(CreateProductForm createProductForm);
		public Task<int> UpdateProductAsync();
		public Task<int> DeleteProductAsync();
		public Task<Products> GetProductByIdAsync(int productId);
		public Task<IEnumerable<Products>> GetAllProductsAsync(); //TODO: nie przewiduję innych potrzeb niż odczyt, więc jest IEnumerable póki co
	}
}
