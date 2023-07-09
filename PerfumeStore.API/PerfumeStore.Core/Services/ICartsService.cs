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
		public Task<Cart?> AddProductToCartAsync(int productId, int userId, decimal productQuantity);
		public Task<Cart> GetCartByIdAsync(int cartId);
		public Task<Cart> DeleteProductLineFromCartAsync(int productId, int userId);
		public Task<Cart> DecreaseProductQuantityAsync(int productId, int userId);
		public Task<Cart> IncreaseProductQuantityAsync(int productId, int userId);
		public Task<Cart> SetProductQuantityAsync(int productId, decimal productQuantity, int userId);
		public Task<CheckCartForm> CheckCartAsync(int userId);
		public Task<Cart> ClearCartAsync(int userId);
	}
}
