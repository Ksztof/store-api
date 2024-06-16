using PerfumeStore.Application.Abstractions.Result.Shared;

namespace PerfumeStore.Application.Abstractions.Result.Authentication
{
    public static class UserErrors
    {
        public static readonly Error MissingCartIdCookie = Error.Authentication("User.MissingCookieWithCartId", "Cookie with cart Id is missing");

        public static readonly Error CantAuthenticateByCartIdOrUserCookie = Error.Authentication("User.CantAuthenticateByCartIdOrUserCookie", "User is not authenticated and cookie with cart Id is missing");

        public static readonly Error InvalidCredentials = Error.Authentication("User.InvalidCredentials", "Login failed. Check your username and password and try again.");
       
        public static readonly Error EmailNotConfirmed = Error.Authentication("User.EmailNotConfirmed", "Account is not activated, please check your email and activate your account with activation link");
       
        public static readonly Error UserDoesntExist = Error.NotFound("User.UserDoesntExist", "User doesn't exist, please create account before you log in");

        public static readonly Error UnableToSetCookieWithJwtToken = Error.Server("User.UnableToSetCookieWithJwtToken", "There was a problem with sending cookie with JWT token");

        public static readonly Error EmailAlreadyTaken = Error.Conflict("User.EmailAlreadyTaken", "Email is already in use.");

        public static readonly Error CantAuthenticateMissingJwtUserIdClaim = Error.Authentication("User.CantAuthenticateMissingJwtUserIdClaim", "Can't find claim with store user Id in token");

        public static readonly Error NotRequestedForAccountDeletion = Error.Validation("User.NotRequestedForAccountDeletion", "the user has not requested to delete the account");
      
        public static Error CantFindUserById(string userId) => Error.NotFound("User.CantFindUserById", $"the user with Id: {userId} cannot be found");
      
        public static Error CantConfirmEmail(IEnumerable<string> errors) => Error.Validation("User.CantConfirmEmail", $"Email confirmation failed with following identity errors: {errors}");
      
        public static Error IdentityErrors(string errors) => Error.Validation("User.IdentityError", $"{errors}");
    }
}
