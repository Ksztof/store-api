using PerfumeStore.Domain.Repositories.Generics;

namespace PerfumeStore.Application.CustomExceptions
{
    public class EntityNotFoundEx<T, TId> : Exception where T : IEntity<TId>
    {
        public TId Id { get; }
        public string Ids { get; }

        public EntityNotFoundEx(TId id) : base($"There is no entity of type {typeof(T)} with id = {id}")
        {
            Id = id;
        }

        public EntityNotFoundEx(TId id, Exception ex) : base($"There is no entity of type {typeof(T)} with id = {id}", ex)
        {
            Id = id;
        }
        public EntityNotFoundEx(string ids)
             : base($"There is no entity of type {typeof(T)} with ids = {ids}")
        {
            Ids = ids;
        }

        public EntityNotFoundEx(string ids, Exception ex)
          : base($"There is no entity of type {typeof(T)} with ids = {ids}", ex)
        {
            Ids = ids;
        }
    }
}