using PerfumeStore.Domain.Interfaces;

namespace PerfumeStore.Core.GenericInterfaces
{
    public interface IRepository<T> where T : IEntity
    {
        public Task<T> CreateAsync(T item);
        public Task<T?> GetByIdAsync(int id);
        public Task<IEnumerable<T>> GetAllAsync();
        public Task<T> UpdateAsync(T item);
        public Task DeleteAsync(int id);
    }
}
