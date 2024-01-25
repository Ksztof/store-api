using PerfumeStore.Application.Abstractions.Result.Shared;

namespace PerfumeStore.Application.Abstractions.Result.Authentication
{
    public static class AuthenticationErrors
    {
        public static readonly Error MissingCartIdCookie = new("Authentication.MissingCookieWithCartId", "Cookie with cart Id is missing");

        public static readonly Error UserNotAuthenticated = new("Authentication.UserNotAuthenticated", "User is not authenticated");

        public static readonly Error MissingCartIdCookieUserNotAuthenticated = new("Authentication.MissingCartIdCookieUserNotAuthenticated", "User is not authenticated and cookie with cart Id is missing");

        public static readonly Error UserDoesNotExist = new("Authentication.UserDoesntExist", "User doesn't exist, please create account before you log in");

        public static readonly Error InvalidCredentials = new("Authentication.InvalidCredentials", "Login failed. Check your username and password and try again.");
       
        public static readonly Error EmailNotConfirmed = new("Authentication.EmailNotConfirmed", "Account is not activated, please check your email and activate your account with activation link");
       
        public static readonly Error UserDoesntExist = new("Authentication.UserDoesntExist", "User doesn't exist, please create account before you log in");

        public static readonly Error UnableToSetCookieWithJwtToken = new("Authentication.UnableToSetCookieWithJwtToken", "There was a problem with sending cookie with JWT token");

        public static readonly Error EmailAlreadyTaken = new("Authentication.EmailAlreadyTaken", "Email is already in use.");

        public static readonly Error MissingUserIdClaim = new("Authentication.MissingUserIdClaim", "Can't find claim with store user Id in token");

        public static readonly Error NotRequestedForAccountDeletion = new("Authentication.NotRequestedForAccountDeletion", "the user has not requested to delete the account");
      
        public static Error CantFindUserById(string userId) => new("Authentication.CantFindUserById", $"the user with Id: {userId} cannot be found");
      
        public static Error CantConfirmEmail(IEnumerable<string> errors) => new("Authentication.CantConfirmEmail", $"Email confirmation failed with following identity errors: {errors}");
      
        public static Error IdentityErrors(string errors) => new("Authentication.IdentityError", $"{errors}");
    }
}
