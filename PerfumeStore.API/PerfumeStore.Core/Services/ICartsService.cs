using PerfumeStore.Core.GenericInterfaces;
using PerfumeStore.Core.ResponseForms;
using PerfumeStore.Domain.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Core.Services
{
	public interface ICartsService
    {
		public Task<Cart?> AddProductToCartAsync(int productId, decimal productQuantity);
		public Task<Cart> GetCartByIdAsync(int cartId);
		public Task<Cart> DeleteProductLineFromCartAsync(int productId);
		public Task<Cart> DecreaseProductQuantityAsync(int productId);
		public Task<Cart> IncreaseProductQuantityAsync(int productId);
		public Task<Cart> SetProductQuantityAsync(int productId, decimal productQuantity);
		public Task<CheckCartForm> CheckCartAsync();
		public Task<Cart> ClearCartAsync();
	}
}
