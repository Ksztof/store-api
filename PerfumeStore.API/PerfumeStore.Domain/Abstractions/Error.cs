using PerfumeStore.Domain.Abstractions;
using PerfumeStore.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Domain.Abstractions
{
    public sealed record Error(string Code, string? Description = null)
    {
        public static readonly Error None = new(string.Empty);

       /* public static implicit operator Result<Error>(Error error) => Result<Error>.Failure(error);*/
    }
}
