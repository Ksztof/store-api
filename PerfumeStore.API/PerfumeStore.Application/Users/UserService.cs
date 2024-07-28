using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using PerfumeStore.Application.Abstractions.Result.Authentication;
using PerfumeStore.Application.Abstractions.Result.Entity;
using PerfumeStore.Application.Abstractions.Result.Shared;
using PerfumeStore.Application.Carts;
using PerfumeStore.Application.Contracts.Email;
using PerfumeStore.Application.Contracts.Guest;
using PerfumeStore.Application.Contracts.HttpContext;
using PerfumeStore.Application.Contracts.JwtToken;
using PerfumeStore.Application.Shared.DTO;
using PerfumeStore.Application.Shared.DTO.Request;
using PerfumeStore.Application.Shared.DTO.Response;
using PerfumeStore.Domain.Entities.Carts;
using PerfumeStore.Domain.Entities.Orders;
using PerfumeStore.Domain.Entities.StoreUsers;
using PerfumeStore.Domain.Repositories;
using System.Data;
using System.Text;
using String = System.String;

namespace PerfumeStore.Application.Users
{
    public class UserService : IUserService
    {
        private readonly UserManager<StoreUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly IHttpContextService _httpContextService;
        private readonly IGuestSessionService _guestSessionService;
        private readonly ICartsService _cartsService;
        private readonly IPermissionService _permissionService;
        private readonly ICartsRepository _cartsRepository;
        private readonly IOrdersRepository _ordersRepository;

        public UserService(
            IMapper mapper,
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
            _mapper = mapper;
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

            string issueResult = await _tokenService.IssueJwtToken(user);

            if (issueResult == string.Empty)
            {
                return UserResult.Failure(UserErrors.UnableToSetCookieWithJwtToken);
            }

            Result<int> receiveCartIdResult = _guestSessionService.GetCartId();

            if (receiveCartIdResult.IsFailure)
            {
                return UserResult.Failure(receiveCartIdResult.Error);
            }

            EntityResult<CartResponse> result = await _cartsService.AssignGuestCartToUserAsync(user.Id, receiveCartIdResult.Value);

            if (result.IsFailure)
            {
                return UserResult.Failure(result.Error);
            }

            _guestSessionService.SetCartIdCookieAsExpired();

            return UserResult.Success();
        }

        public async Task<UserResult> RegisterUser(RegisterUserDtoApp userForRegistration)
        {
            StoreUser storeUser = userForRegistration.StoreUser;

            var userExists = await _userManager.FindByEmailAsync(storeUser.Email);

            if (userExists != null)
            {
                return UserResult.Failure(UserErrors.EmailAlreadyTaken);
            }

            var result = await _userManager.CreateAsync(storeUser, userForRegistration.Password);

            if (!result.Succeeded)
            {
                IEnumerable<string> errors = result.Errors.Select(e => e.Description);
                string errorMessage = String.Join(", ", errors);

                return UserResult.Failure(UserErrors.IdentityErrors(errorMessage));
            }

            Result<int> receiveCartIdResult = _guestSessionService.GetCartId();

            if (receiveCartIdResult.IsFailure)
            {
                return UserResult.Failure(receiveCartIdResult.Error);
            }

            EntityResult<CartResponse> assignResult = await _cartsService.AssignGuestCartToUserAsync(storeUser.Id, receiveCartIdResult.Value);

            if (assignResult.IsFailure)
            {
                return UserResult.Failure(assignResult.Error);
            }

            await _permissionService.AssignVisitorRoleAsync(storeUser);

            UserDetailsForActivationLinkDto userDetails = CreateUserDetailsForActivationLink(storeUser);

            string encodedToken = await GenerateEncodedEmailConfirmationTokenAsync(storeUser);

            await _emailService.SendActivationLink(userDetails, encodedToken);

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
                return UserResult.Failure(UserErrors.WrongAccountActivationToken(token));

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
            var userId = _httpContextService.GetUserId();

            if (string.IsNullOrEmpty(userId))
                return UserResult.Failure(UserErrors.CantAuthenticateMissingJwtUserIdClaim);

            var user = await _userManager.FindByIdAsync(userId);

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

        public async Task<UserResult> SubmitDeletion(string Id)
        {
            StoreUser user = await _userManager.FindByIdAsync(Id);

            if (user is null)
                return UserResult.Failure(UserErrors.UserDoesntExist);

            Cart? cart = await _cartsRepository.GetByUserIdAsync(Id);

            if (cart != null)
                await _cartsRepository.DeleteAsync(cart);

            IEnumerable<Order> orders = await _ordersRepository.GetByUserIdAsync(Id);

            if (orders.Any())
                await _ordersRepository.DeleteOrdersAsync(orders);

            if (user.IsDeleteRequested is not true)
                return UserResult.Failure(UserErrors.NotRequestedForAccountDeletion);

            var deleteResult = await _userManager.DeleteAsync(user);

            if (!deleteResult.Succeeded)
            {
                IEnumerable<string> errors = deleteResult.Errors.Select(e => e.Description);
                string errorMessage = String.Join(", ", errors);

                return UserResult.Failure(UserErrors.IdentityErrors(errorMessage));
            }

            return UserResult.Success();
        }

        public UserResult Logout()
        {
            throw new NotImplementedException();

        }

        public UserResult RemoveGuestSessionId()
        {
            UserResult result = _guestSessionService.SetCartIdCookieAsExpired();

            return result;
        }
    }
}
