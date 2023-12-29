using AutoMapper;
using EllipticCurve.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using PerfumeStore.Application.Carts;
using PerfumeStore.Application.Cookies;
using PerfumeStore.Application.Core;
using PerfumeStore.Application.DTOs;
using PerfumeStore.Application.DTOs.Request;
using PerfumeStore.Application.DTOs.Response;
using PerfumeStore.Application.HttpContext;
using PerfumeStore.Domain.Abstractions;
using PerfumeStore.Domain.Carts;
using PerfumeStore.Domain.Errors;
using PerfumeStore.Domain.Orders;
using PerfumeStore.Domain.StoreUsers;
using PerfumeStore.Domain.Tokens;
using System.Data;
using System.Security.Claims;
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

        public async Task<AuthenticationResult> Login(AuthenticateUserDtoApp userForAuthentication)
        {
            StoreUser user = await _userManager.FindByEmailAsync(userForAuthentication.Email);

            if (user == null)
            {
                return AuthenticationErrors.UserDoesntExist;
            }

            if (!await _userManager.CheckPasswordAsync(user, userForAuthentication.Password))
            {
                return AuthenticationErrors.InvalidCredentials;
            }

            if (!user.EmailConfirmed)
            {
                return AuthenticationErrors.EmailNotConfirmed;
            }

            var tokenResponse = await _tokenService.GetToken(user);

            if (tokenResponse == string.Empty)
            {
                return AuthenticationErrors.UnableToGetToken;
            }

            int? cartId = _guestSessionService.GetCartId();

            if (cartId is not null)
            {
                EntityResult<CartResponse> result = await _cartsService.AssignCartToUserAsync(user.Id, cartId.Value);

                if (result.IsFailure)
                {
                    return AuthenticationResult.Failure(result.Error);
                }
            }

            _guestSessionService.SetCartIdCookieAsExpired();

            return AuthenticationResult.Success(tokenResponse);
        }

        public async Task<AuthenticationResult> RegisterUser(RegisterUserDtoApp userForRegistration)
        {
            StoreUser storeUser = userForRegistration.StoreUser;

            var userExists = await _userManager.FindByEmailAsync(storeUser.Email);

            if (userExists != null)
            {
                return AuthenticationErrors.EmailAlreadyTaken;
            }

            var result = await _userManager.CreateAsync(storeUser, userForRegistration.Password);

            if (!result.Succeeded)
            {
                IEnumerable<string> errors = result.Errors.Select(e => e.Description);
                string errorMessage = String.Join(", ", errors);

                return AuthenticationErrors.IdentityErrors(errorMessage);
            }

            int? cartId = _guestSessionService.GetCartId();

            if (cartId != null)
            {

                EntityResult<CartResponse> assignResult = await _cartsService.AssignCartToUserAsync(storeUser.Id, cartId.Value);

                if (assignResult.IsFailure)
                {
                    return AuthenticationResult.Failure(assignResult.Error);
                }
            }

            await _permissionService.AssignVisitorRoleAsync(storeUser);

            UserDetailsForActivationLinkDto userDetails = CreateUserDetailsForActivationLink(storeUser);

            string encodedToken = await GenerateEncodedEmailConfirmationTokenAsync(storeUser);

            await _emailService.SendActivationLink(userDetails, encodedToken);

            return AuthenticationResult.Success();
        }

        public async Task<string> GenerateEncodedEmailConfirmationTokenAsync(StoreUser user)
        {
            string? token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            string? encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            return encodedToken;
        }

        public async Task<AuthenticationResult> ConfirmEmail(string userId, string emailToken)
        {
            string decodedToken = _emailService.DecodeBaseUrlToken(emailToken);

            AuthenticationResult findUser = await FindByIdAsync(userId);
            StoreUser storeUser = findUser.StoreUser;

            var result = await _userManager.ConfirmEmailAsync(storeUser, decodedToken);

            if (!result.Succeeded)
            {
                IEnumerable<string> errors = result.Errors.Select(x => x.Description);

                return AuthenticationErrors.CantConfirmEmail(errors);
            }

            return AuthenticationResult.Success();
        }

        public async Task<AuthenticationResult> FindByIdAsync(string userId)
        {
            StoreUser user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                AuthenticationErrors.CantFindUserById(userId);

            return AuthenticationResult.Success(user);
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

        public async Task<AuthenticationResult> RequestDeletion()
        {
            var userId = _httpContextService.GetUserId();

            if (string.IsNullOrEmpty(userId))
                return AuthenticationErrors.MissingUserIdClaim;

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null || user.IsDeleteRequested is true)
                return AuthenticationErrors.UserDoesntExist;

            user.IsDeleteRequested = true;

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                IEnumerable<string> errors = updateResult.Errors.Select(e => e.Description);
                string errorMessage = String.Join(", ", errors);

                return AuthenticationErrors.IdentityErrors(errorMessage);
            }

            return AuthenticationResult.Success();
        }

        public async Task<AuthenticationResult> SubmitDeletion(string Id)
        {
            StoreUser user = await _userManager.FindByIdAsync(Id); 

            if (user is null)
                return AuthenticationErrors.UserDoesntExist;

            Cart? cart = await _cartsRepository.GetByUserIdAsync(Id);

            if (cart != null)
                await _cartsRepository.DeleteAsync(cart);

            IEnumerable<Order> orders = await _ordersRepository.GetByUserIdAsync(Id);

            if(orders.Any())
                await _ordersRepository.DeleteOrdersAsync(orders);

            if (user.IsDeleteRequested is not true)
                return AuthenticationErrors.NotRequestedForAccountDeletion;

            var deleteResult = await _userManager.DeleteAsync(user);

            if (!deleteResult.Succeeded)
            {
                IEnumerable<string> errors = deleteResult.Errors.Select(e => e.Description);
                string errorMessage = String.Join(", ", errors);

                return AuthenticationErrors.IdentityErrors(errorMessage);
            }

            return AuthenticationResult.Success();
        }
    }
}
