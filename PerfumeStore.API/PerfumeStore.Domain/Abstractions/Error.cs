using PerfumeStore.Domain.Abstractions;
using PerfumeStore.Domain.EnumsEtc;
using PerfumeStore.Domain.Interfaces;
using PerfumeStore.Domain.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Domain.Abstractions
{
    public sealed record Error(string code, string? description = null)
    {
        public static readonly Error None = new(string.Empty);
        public static implicit operator AuthenticationResult(Error error) => AuthenticationResult.Failure(error);
    }
}
