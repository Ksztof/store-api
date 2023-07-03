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
		public int CreateAsync(T item);
		public T GetByIdAsync(int id);
		public Task<IEnumerable<T>> GetAllAsync();
		public void UpdateAsync(T item);
		public void DeleteAsync(int id);
	}
}
