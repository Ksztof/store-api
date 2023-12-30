namespace PerfumeStore.Domain.Repositories.Generics
{
    public interface IRepository<T, TId> where T : IEntity<TId>
    {
        public Task<T> CreateAsync(T item);

        public Task<T?> GetByIdAsync(TId id);

        public Task<IEnumerable<T>> GetAllAsync();

        public Task<T> UpdateAsync(T item);

        public Task DeleteAsync(TId id);
    }
}
