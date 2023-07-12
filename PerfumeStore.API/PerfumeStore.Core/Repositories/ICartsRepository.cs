using PerfumeStore.Core.GenericInterfaces;
using PerfumeStore.Domain.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Core.Repositories
{
	public interface ICartsRepository
	{
		public Task<Cart?> CheckIfCartExists(Guid userId);
		public Task<Cart> CreateAsync(Cart item);
		public Task<Cart> UpdateAsync(Cart item);
		public Task<Cart> GetByCartIdAsync(int cartId);
		//public Task<Cart> GetByUserIdAsync(int userId);
		public Task<Cart> GetByUserGuidIdAsync(Guid userGuidId);


	}
}
