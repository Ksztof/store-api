using PerfumeStore.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Core.GenericInterfaces
{
	public interface IRepository<T> where T : IEntity
	{
		public Task<int> CreateAsync(T item);
		public Task<T> GetByIdAsync(int id);
		public Task<IEnumerable<T>> GetAllAsync();
		public Task<T> UpdateAsync(T item);
		public Task DeleteAsync(int id);
	}
}
