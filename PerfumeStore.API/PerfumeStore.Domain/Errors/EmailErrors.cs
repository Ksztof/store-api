using PerfumeStore.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Domain.Errors
{
    public static class EmailErrors
    {
        public static readonly Error MissingCartIdCookie = new("Authentication.MissingCookieWithCartId", "Cookie with cart Id is missing");
    }
}
