using PerfumeStore.Domain.Abstractions;
using PerfumeStore.Domain.EnumsEtc;
using PerfumeStore.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Domain.Errors
{
    public static class AuthenticationErrors
    {
        public static readonly Error MissingCartIdCookie = new("Authentication.MissingCookieWithCartId", "Cookie with cart Id is missing");

        public static readonly Error UserNotAuthenticated = new("Authentication.UserNotAuthenticated", "User is not authenticated");

        public static readonly Error MissingCartIdCookieUserNotAuthenticated = new("Authentication.MissingCartIdCookieUserNotAuthenticated", "User is not authenticated and cookie with cart Id is missing");

        public static readonly Error UserDoesntExist = new("Authentication.UserDoesntExist", "User doesn't exist, please create account before you log in");

        public static readonly Error InvalidCredentials = new("Authentication.InvalidCredentials", "Login failed. Check your username and password and try again.");
        public static readonly Error EmailNotConfirmed = new("Authentication.EmailNotConfirmed", "Account is not activated, please check your email and activate your account with activation link");

        public static readonly Error UnableToGetToken = new("Authentication.UnableToGetToken", "There were issues during token generation.");
    }
}
