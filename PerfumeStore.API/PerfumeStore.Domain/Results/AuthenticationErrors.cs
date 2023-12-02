using PerfumeStore.Domain.Abstractions;
using PerfumeStore.Domain.EnumsEtc;
using PerfumeStore.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Domain.Results
{
    public static class AuthenticationErrors
    {
        public static readonly Error MissingCartIdCookie = new($"Authentication.MissingCookieWithCartId", "Cookie with cart Id is missing");

        public static readonly  Error UserNotAuthenticated = new($"Authentication.UserNotAuthenticated", "User is not authenticated");

        public static readonly Error MissingCartIdCookieUserNotAuthenticated = new($"Authentication.MissingCartIdCookieUserNotAuthenticated", "User is not authenticated and cookie with cart Id is missing");
    }
}
