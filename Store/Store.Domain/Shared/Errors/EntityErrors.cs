using Store.Domain.Abstractions;

namespace Store.Domain.Shared.Errors
{
    public static class EntityErrors<T, TId> where T : Entity
    {
        public static Error NotFound(TId entityId) => Error.NotFound(
            $"{typeof(T).Name}.NotFound", $"Entity with ID: {entityId} is missing");

        public static Error NotFoundByProductId(int productId) => Error.NotFound(
            $"{typeof(T).Name}.NotFoundByProductId", $"Entity with product ID: {productId} is missing");

        public static Error NotFoundByCartId(int cartId) => Error.NotFound(
            $"{typeof(T).Name}.NotFoundByCartId", $"Entity with cart ID: {cartId} is missing");

        public static Error NotFoundByOrderId(int orderId) => Error.NotFound(
            $"{typeof(T).Name}.NotFoundByOrderId", $"Entity with order ID: {orderId} is missing");

        public static Error NotFoundByUserId(string userId) => Error.NotFound(
           $"{typeof(T).Name}.NotFoundByUserId", $"For user with Id = {userId} the entity is missing");

        public static Error NotFoundEntitiesByIds(IEnumerable<TId> entityIds) => Error.NotFound(
            $"{typeof(T).Name}.NotFoundEntitiesByIds", $"Entities with IDs: {string.Join(", ", entityIds)} are missing");

        public static Error EntityDoesntBelongToYou(TId entityId) => Error.Authorization(
            $"{typeof(T).Name}.EntityDoesntBelongToYou", $"Entity with ID: {entityId} does not belong to user trying to modify it");

        public static Error EntityInUse(TId entityId, int cartId) => Error.Conflict(
            $"{typeof(T).Name}.EntityInUse", $"Cart with id: {cartId} has been used in {typeof(T).Name} with id : {entityId}");

        public static Error ProductAlreadyExists(TId entityId, string productName) => Error.Conflict(
            $"{typeof(T).Name}.ProductAlreadyExists", $"Product with id: {entityId} and name: {productName} already exists.");

        public static Error WrongEntityId(TId entityId) => Error.Validation(
            $"{typeof(T).Name}.WrongEntityId", $"Entity ID of type: {typeof(T).Name} is smaller than 1. ID = {entityId}");

        public static Error ParsingEntityIdFailed() => Error.Validation(
            $"{typeof(T).Name}.ParsingFailed", $"Entity ID of type: {typeof(T).Name} can't be parsed");

        public static Error EntityIdNotMatch(int incomingId, int id) => Error.Validation(
            $"{typeof(T).Name}.EntityIdNotMatch", $"Entity ID of type: {typeof(T).Name} doesn't match to ID of the compared object, incoming ID: {incomingId}, existing ID: {id} ");
    }
}
