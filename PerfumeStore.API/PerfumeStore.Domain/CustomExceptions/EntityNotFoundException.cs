using PerfumeStore.Domain.Interfaces;

namespace PerfumeStore.Domain.CustomExceptions
{
    public class EntityNotFoundException<T, TId> : Exception where T : IEntity<TId>
    {
        public TId Id { get; }
        public string Ids { get; }

        public EntityNotFoundException(TId id) : base($"There is no entity of type {typeof(T)} with id = {id}")
        {
            Id = id;
        }

        public EntityNotFoundException(TId id, Exception ex) : base($"There is no entity of type {typeof(T)} with id = {id}", ex)
        {
            Id = id;
        }
        public EntityNotFoundException(string ids)
             : base($"There is no entity of type {typeof(T)} with ids = {ids}")
        {
            Ids = ids;
        }

        public EntityNotFoundException(string ids, Exception ex)
          : base($"There is no entity of type {typeof(T)} with ids = {ids}", ex)
        {
            Ids = ids;
        }
    }
}