using PerfumeStore.Domain.Interfaces;

namespace PerfumeStore.Core.CustomExceptions
{
    public class EntityNotFoundException<T, TId> : Exception where T : IEntity<TId>
    {
        public EntityNotFoundException(string message) : base(message) { }
        public EntityNotFoundException(string message, Exception ex) : base(message, ex) { }
    }
}
