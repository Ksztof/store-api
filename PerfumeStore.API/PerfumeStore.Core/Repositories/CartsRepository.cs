using PerfumeStore.Domain;
using PerfumeStore.Domain.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Core.Repositories
{
    public class CartsRepository : ICartsRepository
    {
        public async Task<Cart?> CheckIfCartExists(Guid userId)
        {
            Cart? cart = InMemoryDatabase.carts.FirstOrDefault(c => c.UserId == userId);
            if (cart == null)
            {
                return null;
            }

            return await Task.FromResult(cart);
        }

        public async Task<Cart> CreateAsync(Cart item)
        {
            item.CartId = CartIdGenerator.GetNextId();
            InMemoryDatabase.carts.Add(item);

            return await Task.FromResult(item);   
        }

		public async Task<Cart> UpdateAsync(Cart item)
        {
            int cartToUpdateIndex = InMemoryDatabase.carts.IndexOf(item);
            InMemoryDatabase.carts[cartToUpdateIndex] = item;

            return await Task.FromResult(item);
        }

		public async Task<Cart> GetByCartIdAsync(int cartId)
		{
			Cart cart = InMemoryDatabase.carts.FirstOrDefault(x => x.CartId == cartId);

			return await Task.FromResult(cart);
		}

		public async Task<Cart> GetByUserIdAsync(int userId)
		{
			Cart cart = InMemoryDatabase.carts.FirstOrDefault(x => x.UserId == userId);

			return await Task.FromResult(cart);
		}
    }
}
