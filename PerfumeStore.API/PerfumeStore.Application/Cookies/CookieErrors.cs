using PerfumeStore.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Application.Cookies
{
    public static class CookieErrors
    {
        public static readonly Error MissingCookie = new("Cookies.MissingCookie", "Can't find cookie");
    }
}
