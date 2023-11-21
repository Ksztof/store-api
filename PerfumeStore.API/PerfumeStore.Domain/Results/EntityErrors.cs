using PerfumeStore.Domain.Abstractions;
using PerfumeStore.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Domain.Results
{
    public static class EntityErrors<T, TId> where T : IEntity<TId>
    {
        public static Error MissingEntity(TId entityId)
        {
            return new($"{typeof(T).Name}.MissingEntity", $"Entity with ID: {entityId} is missing");
        }

        public static Error MissingEntities(IEnumerable<TId> entityIds)
        {
            var ids = string.Join(", ", entityIds);
            return new Error($"{typeof(T).Name}.MissingEntities", $"Entities with IDs: {ids} are missing");
        }
    }
}
