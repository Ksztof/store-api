using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Store.Application.Carts;
using Store.Application.Carts.Dto.Response;
using Store.Application.Contracts.ContextHttp;
using Store.Application.Contracts.Email;
using Store.Application.Contracts.Guest;
using Store.Application.Contracts.JwtToken;
using Store.Application.Users.Dto;
using Store.Application.Users.Dto.Request;
using Store.Domain.Abstractions;
using Store.Domain.Carts;
using Store.Domain.Orders;
using Store.Domain.StoreUsers;
using Store.Domain.StoreUsers.Errors;
using System.Data;
using System.Text;
using String = System.String;

namespace Store.Application.Users
{
    public class UserService : IUserService
    {
        private readonly UserManager<StoreUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly ITokenService _tokenService;
        private readonly IHttpContextService _httpContextService;
        private readonly IGuestSessionService _guestSessionService;
        private readonly ICartsService _cartsService;
        private readonly IPermissionService _permissionService;
        private readonly ICartsRepository _cartsRepository;
        private readonly IOrdersRepository _ordersRepository;

        public UserService(
            UserManager<StoreUser> userManager,
            IEmailService emailService,
            ITokenService tokenService,
            IHttpContextService httpContextService,
            IGuestSessionService guestSessionService,
            ICartsService cartsService,
            IPermissionService permissionService,
            ICartsRepository cartsRepository,
            IOrdersRepository ordersRepository)
        {
            _userManager = userManager;
            _emailService = emailService;
            _tokenService = tokenService;
            _guestSessionService = guestSessionService;
            _cartsService = cartsService;
            _permissionService = permissionService;
            _httpContextService = httpContextService;
            _cartsRepository = cartsRepository;
            _ordersRepository = ordersRepository;
        }

        public async Task<UserResult> Login(AuthenticateUserDtoApp userForAuthentication)
        {
            StoreUser user = await _userManager.FindByEmailAsync(userForAuthentication.Email);

            if (user == null)
            {
                return UserResult.Failure(UserErrors.UserDoesntExist);
            }

            if (!await _userManager.CheckPasswordAsync(user, userForAuthentication.Password))
            {
                return UserResult.Failure(UserErrors.InvalidCredentials);
            }

            if (!user.EmailConfirmed)
            {
                return UserResult.Failure(UserErrors.EmailNotConfirmed);
            }

            Result tokenResult = await _tokenService.IssueJwtToken(user);

            if (tokenResult.IsFailure)
            {
                return UserResult.Failure(tokenResult.Error);
            }

            Result refreshTokenResult = await _tokenService.IssueRefreshToken(user);

            if (refreshTokenResult.IsFailure)
            {
                return UserResult.Failure(refreshTokenResult.Error);
            }

            Result<int> receiveCartIdResult = _guestSessionService.GetCartId();

            if (receiveCartIdResult.IsSuccess)
            {
                EntityResult<CartResponse> result = await _cartsService.AssignGuestCartToUserAsync(user.Id, receiveCartIdResult.Value);

                if (result.IsFailure)
                {
                    return UserResult.Failure(result.Error);
                }
            }

            _guestSessionService.SetCartIdCookieAsExpired();

            return UserResult.Success();
        }

        public async Task<UserResult> RegisterUser(RegisterUserDtoApp userForRegistration)
        {
            StoreUser storeUser = userForRegistration.StoreUser;

            var user = await _userManager.FindByEmailAsync(storeUser.Email);

            if (user != null)
            {
                return UserResult.Failure(UserErrors.EmailAlreadyTaken);
            }

            var result = await _userManager.CreateAsync(storeUser, userForRegistration.Password);

            if (!result.Succeeded)
            {
                //Authentication errors can be chacked like this: == nameof(IdentityErrorDescriber.DefaultError);
                IEnumerable<string> errors = result.Errors.Select(e => e.Description);
                string errorMessage = String.Join(", ", errors);

                return UserResult.Failure(UserErrors.IdentityErrors(errorMessage));
            }

            Result<int> receiveCartIdResult = _guestSessionService.GetCartId();

            if (receiveCartIdResult.IsSuccess)
            {
                EntityResult<CartResponse> assignResult = await _cartsService.AssignGuestCartToUserAsync(storeUser.Id, receiveCartIdResult.Value);

                if (assignResult.IsFailure)
                {
                    return UserResult.Failure(assignResult.Error);
                }
            }

            await _permissionService.AssignVisitorRoleAsync(storeUser);

            UserDetailsForActivationLinkDto userDetails = CreateUserDetailsForActivationLink(storeUser);

            string encodedToken = await GenerateEncodedEmailConfirmationTokenAsync(storeUser);

            Result sendingResult = await _emailService.SendActivationLink(userDetails, encodedToken);

            if (sendingResult.IsFailure)
            {
                return UserResult.Failure(sendingResult.Error);
            }

            return UserResult.Success();
        }

        public async Task<string> GenerateEncodedEmailConfirmationTokenAsync(StoreUser user)
        {
            string? token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            string? encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            return encodedToken;
        }

        public async Task<UserResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
            {
                return UserResult.Failure(UserErrors.WrongAccountActivationToken(token));
            }

            string decodedToken = _emailService.DecodeBaseUrlToken(token);

            UserResult findUser = await FindByIdAsync(userId);
            StoreUser storeUser = findUser.StoreUser;

            var result = await _userManager.ConfirmEmailAsync(storeUser, decodedToken);

            if (!result.Succeeded)
            {
                IEnumerable<string> errors = result.Errors.Select(x => x.Description);

                return UserResult.Failure(UserErrors.CantConfirmEmail(errors));
            }

            return UserResult.Success();
        }

        public async Task<UserResult> FindByIdAsync(string userId)
        {
            StoreUser user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                UserErrors.CantFindUserById(userId);

            return UserResult.Success(user);
        }

        private static UserDetailsForActivationLinkDto CreateUserDetailsForActivationLink(StoreUser user)
        {
            return new UserDetailsForActivationLinkDto
            {
                UserId = user.Id,
                UserEmail = user.Email,
                UserName = user.UserName,
            };
        }

        public async Task<UserResult> RequestDeletion()
        {
            Result<string> result = _httpContextService.GetUserId();

            if (result.IsFailure)
            {
                return UserResult.Failure(result.Error);
            }

            var user = await _userManager.FindByIdAsync(result.Value);

            if (user == null || user.IsDeleteRequested is true)
                return UserResult.Failure(UserErrors.UserDoesntExist);

            user.IsDeleteRequested = true;

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                IEnumerable<string> errors = updateResult.Errors.Select(e => e.Description);
                string errorMessage = String.Join(", ", errors);

                return UserResult.Failure(UserErrors.IdentityErrors(errorMessage));
            }

            return UserResult.Success();
        }

        public async Task<UserResult> SubmitDeletion(string userId)
        {
            StoreUser user = await _userManager.FindByIdAsync(userId);

            if (user.IsDeleteRequested is not true)
                return UserResult.Failure(UserErrors.NotRequestedForAccountDeletion);

            if (user is null)
                return UserResult.Failure(UserErrors.UserDoesntExist);

            EntityResult<Cart> getCart = await _cartsRepository.GetByUserIdAsync(userId);

            if (getCart.IsSuccess)
                await _cartsRepository.DeleteAsync(getCart.Entity);

            IEnumerable<Order> orders = await _ordersRepository.GetByUserIdAsync(userId);

            if (orders.Any())
                await _ordersRepository.DeleteOrdersAsync(orders);

            IdentityResult deleteResult = await _userManager.DeleteAsync(user);

            if (!deleteResult.Succeeded)
            {
                IEnumerable<string> errors = deleteResult.Errors.Select(e => e.Description);
                string errorMessage = String.Join(", ", errors);

                return UserResult.Failure(UserErrors.IdentityErrors(errorMessage));
            }

            return UserResult.Success();
        }

        public Result RemoveAuthToken()
        {
            Result result = _tokenService.RemoveAuthToken();

            return result;
        }

        public Result RemoveRefreshToken()
        {
            Result result = _tokenService.RemoveRefreshToken();

            return result;
        }

        public UserResult RemoveGuestSessionId()
        {
            UserResult result = _guestSessionService.SetCartIdCookieAsExpired();

            return result;
        }
    }
}
