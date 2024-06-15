using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Application.Abstractions.Result.Shared
{
    public enum ErrorCode
    {
        None,
        Authentication,
        Authorization,
        Validation,
        Server,
        NotFound
    }
}
