using Store.Domain.Abstractions;

namespace Store.Domain.StoreUsers.Errors
{
    public static class UserErrors
    {
        public static readonly Error MissingCartIdCookie = Error.Authentication("User.MissingCookieWithCartId", "Cookie with cart Id is missing");

        public static readonly Error CantAuthenticateByCartIdOrUserCookie = Error.Authentication("User.CantAuthenticateByCartIdOrUserCookie", "User is not authenticated and cookie with cart Id is missing");

        public static readonly Error InvalidCredentials = Error.Authentication("UserValidation.InvalidCredentials", "Login failed. Check your username and password and try again.");

        public static readonly Error UserNotAuthenticated = Error.Authentication("User.UserNotAuthenticated", "Failed to authenticate user");

        public static readonly Error EmailNotConfirmed = Error.Authentication("UserValidation.EmailNotConfirmed", "Account is not activated, please check your email and activate your account with activation link");

        public static readonly Error UserDoesntExist = Error.NotFound("UserValidation.UserDoesntExist", "User doesn't exist, please create account before you log in");

        public static readonly Error UnableToSetCookieWithJwtToken = Error.Server("User.UnableToSetCookieWithJwtToken", "There was a problem with sending cookie with JWT token");

        public static readonly Error EmailAlreadyTaken = Error.Conflict("UserValidation.EmailAlreadyTaken", "Email is already in use.");

        public static readonly Error CantAuthenticateMissingJwtUserIdClaim = Error.Authentication("User.CantAuthenticateMissingJwtUserIdClaim", "Can't find claim with store user Id in token");

        public static readonly Error FailedToUpdateUserWithRefreshToken = Error.Authentication("User.FailedToUpdateUserWithRefreshToken", "User can't be updated and can't assign refresh token");

        public static readonly Error NotRequestedForAccountDeletion = Error.Validation("User.NotRequestedForAccountDeletion", "the user has not requested to delete the account");

        public static Error CantFindUserById(string userId) => Error.NotFound("User.CantFindUserById", $"the user with Id: {userId} cannot be found");

        public static Error CantConfirmEmail(IEnumerable<string> errors) => Error.Validation("User.CantConfirmEmail", $"Email confirmation failed with following identity errors: {errors}");

        public static Error IdentityErrors(string errors) => Error.Validation("UserValidation.IdentityError", $"{errors}");


        public static Error WrongAccountActivationToken(string token) => Error.Validation("User.WrongAccountActivationToken", $"Token used during account activation via email activation link is wrong. Token value: {token}");
        public static readonly Error MissingHttpContext = Error.Server("User.MissingHttpContext", "Http context is missing");
        public static readonly Error MissingGuestSessionId = Error.Server("User.MissingGuestSessionId", "Guest session ID is missing in GuestSessionId cookie.");
        public static readonly Error MissingHttpProtocol = Error.Server("User.MissingHttpProtocol", "Unable to send activation link because of missing protocol / http Request.Scheme");
        public static readonly Error MissingNameIdentifier = Error.Server("User.MissingNameIdentifier", "Unable to get user Id because of missing name identifier in http context");

    }
}
