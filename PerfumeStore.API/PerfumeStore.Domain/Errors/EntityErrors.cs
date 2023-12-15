using PerfumeStore.Domain.Abstractions;
using PerfumeStore.Domain.Interfaces;

namespace PerfumeStore.Domain.Errors
{
    public static class EntityErrors<T, TId> where T : IEntity<TId>
    {
        public static Error MissingEntity(TId entityId)
        {
            return new Error($"{typeof(T).Name}.MissingEntity", $"Entity with ID: {entityId} is missing");
        }

        public static Error MissingEntityByUserId(string userId)
        {
            return new Error($"{typeof(T).Name}.MissingEntityByUserId", $"For user with Id = {userId} the entity is missing");
        }
        
        public static Error MissingEntityForGuestId(int cartId)
        {
            return new Error($"{typeof(T).Name}.MissingEntityForGuestId", $"For guest of cartId = {cartId} the entity is missing");
        }

        public static Error MissingEntity(string userId)
        {
            return new Error($"{typeof(T).Name}.MissingEntity", $"Entity for user with ID: {userId} is missing");
        }

        public static Error MissingEntities()
        {
            return new Error($"{typeof(T).Name}.MissingEntities", $"There are no entities");
        }

        public static Error MissingEntities(IEnumerable<TId> entityIds)
        {
            var ids = string.Join(", ", entityIds);
            return new Error($"{typeof(T).Name}.MissingEntities", $"Entities with IDs: {ids} are missing");
        }

        public static Error EntityDoesntBelongToYou(TId entityId)
        {
            return new Error($"{typeof(T).Name}.EntityDoesntBelongToYou", $"Entity with ID: {entityId} does not belong to user trying to modify it");
        }
    }
}
