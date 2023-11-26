using PerfumeStore.Domain.Abstractions;
using PerfumeStore.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Domain.Results
{
    public static class CookieError
    {
        public static Error MissingCookie()
        {
            return new("Cookies.MissingCookie", "Cookie is missing");
        }
    }
}
