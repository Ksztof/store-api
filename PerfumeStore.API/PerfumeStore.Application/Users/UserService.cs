using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PerfumeStore.Application.Carts;
using PerfumeStore.Application.Cookies;
using PerfumeStore.Application.Core;
using PerfumeStore.Application.CustomExceptions;
using PerfumeStore.Application.DTOs.Request;
using PerfumeStore.Application.DTOs.Response;
using PerfumeStore.Domain.Abstractions;
using PerfumeStore.Domain.Carts;
using PerfumeStore.Domain.EnumsEtc;
using PerfumeStore.Domain.Errors;
using PerfumeStore.Domain.StoreUsers;
using PerfumeStore.Domain.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PerfumeStore.Application.Users
{
    public class UserService : IUserService
    {
        private readonly UserManager<StoreUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IConfigurationSection _jwtSettings; //KM skorzystaj z Options Pattern np. JwtSettings klasa, którą uzupełnisz w program.cs z konfiguracji
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenService _tokenService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICookiesService _guestSessionService;
        private readonly ICartsService _cartsService;

        public UserService(IMapper mapper, UserManager<StoreUser> userManager, IConfiguration configuration, IEmailService emailService, RoleManager<IdentityRole> roleManager, ITokenService tokenService, IHttpContextAccessor httpContextAccessor, ICookiesService guestSessionService, ICartsService cartsService)
        {
            _mapper = mapper;
            _userManager = userManager;
            _configuration = configuration;
            _jwtSettings = _configuration.GetSection("JwtSettings");
            _emailService = emailService;
            _roleManager = roleManager;
            _tokenService = tokenService;
            _httpContextAccessor = httpContextAccessor;
            _guestSessionService = guestSessionService;
            _cartsService = cartsService;
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

            return AuthenticationResult.Success(tokenResponse);
        }

        public async Task<RegistrationResponseDto> RegisterUser(RegisterUserDtoApp userForRegistration)
        {
            var userExists = await _userManager.FindByEmailAsync(userForRegistration.StoreUser.Email);
            if (userExists != null)
            {
                return new RegistrationResponseDto { Message = "Email is already taken." };
            }
            
            var result = await _userManager.CreateAsync(userForRegistration.StoreUser, userForRegistration.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return new RegistrationResponseDto { Errors = errors, IsSuccessfulRegistration = false };
            }

            int? cartId = _guestSessionService.GetCartId();
            if (cartId is not null)
            {
                var cart = await _cartsService.GetCartByIdAsync(cartId.Value);
                user.Carts.Add(cart);
            }

            string visitorRole = Roles.Visitor;

            //KM Uważam, że powinieneś mieć klasę PermissionService, która będzie Ci zarządzać uprawnieniami.
            // Mógłbyś mieć wtedy metodę PermissionService.AssignRole(Roles.Visitor), która by Ci obsłużyła kod poniżej
            if (!await _roleManager.RoleExistsAsync(visitorRole))
                await _roleManager.CreateAsync(new IdentityRole(visitorRole));

            if (await _roleManager.RoleExistsAsync(visitorRole))
                await _userManager.AddToRoleAsync(user, visitorRole);

            await _emailService.SendActivationLink(user);

            return new RegistrationResponseDto { IsSuccessfulRegistration = true };
        }

        public async Task<bool> ConfirmEmail(string userId, string emailToken)
        {
            await _emailService.ConfirmEmail(userId, emailToken);
            return true; //TODO: Add error handling etc
        }

        public async Task<bool> RequestDeletion()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(userId))
            {
                throw new MissingClaimInTokenEx(ClaimTypes.NameIdentifier);
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new RequestForUserEx($"User with Id: {userId} - not found.");

            user.IsDeleteRequested = true;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                throw new UserModificationEx("UpdateAsync", user.Id);

            return true;
        }

        public async Task<bool> SubmitDeletion(string Id)
        {
            StoreUser user = await _userManager.FindByIdAsync(Id);
            if (user is null)
            {
                throw new RequestForUserEx("Can't find user");
            }

            if (user.IsDeleteRequested is not true)
            {
                throw new InvalidOperationException("You can't delete active user");
            }

            var deleteResul = await _userManager.DeleteAsync(user);
            if (!deleteResul.Succeeded)
                throw new UserModificationEx("DeleteAsync", user.Id);

            return true;
        }
    }
}
