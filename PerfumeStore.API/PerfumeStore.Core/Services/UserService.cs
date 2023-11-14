using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PerfumeStore.Core.CustomExceptions;
using PerfumeStore.Core.DTOs.Request;
using PerfumeStore.Core.DTOs.Response;
using PerfumeStore.Domain.DbModels;
using PerfumeStore.Domain.EnumsEtc;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PerfumeStore.Core.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<StoreUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IConfigurationSection _jwtSettings;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenService _tokenService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGuestSessionService _guestSessionService;
        private readonly ICartsService _cartsService;

        public UserService(IMapper mapper, UserManager<StoreUser> userManager, IConfiguration configuration, IEmailService emailService, RoleManager<IdentityRole> roleManager, ITokenService tokenService, IHttpContextAccessor httpContextAccessor, IGuestSessionService guestSessionService, ICartsService cartsService)
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

        public async Task<AuthResponseDto> Login(UserForAuthenticationDto userForAuthentication)
        {
            var user = await _userManager.FindByEmailAsync(userForAuthentication.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, userForAuthentication.Password))
            {
                AuthResponseDto failedResponse = new AuthResponseDto { ErrorMessage = "Invalid Authentication" };
                return failedResponse;
            }

            if (!user.EmailConfirmed)
            {
                return new AuthResponseDto { ErrorMessage = "Please activate your account" };
            }

            var tokenResponse = await _tokenService.GetToken(user);

            if (tokenResponse == string.Empty)
            {
                AuthResponseDto failedResponse = new AuthResponseDto { ErrorMessage = "Error obtaining token" };
                return failedResponse;
            }

            int? cartId = _guestSessionService.GetCartId();
            if (cartId is not null)
            {
                var cart = await _cartsService.GetCartByIdAsync(cartId.Value);
                user.Carts.Add(cart);
                var updateUser = await _userManager.UpdateAsync(user);
                if (!updateUser.Succeeded)
                    throw new UserModificationException("UpdateAsync", user.Id);
            }

            AuthResponseDto authResponse = new AuthResponseDto
            {
                IsAuthSuccessful = true,
                Token = tokenResponse
            };

            return authResponse;
        }

        public async Task<RegistrationResponseDto> RegisterUser(UserForRegistrationDto userForRegistration)
        {
            var userExists = await _userManager.FindByEmailAsync(userForRegistration.Email);
            if (userExists != null)
            {
                return new RegistrationResponseDto { Message = "Email is already taken." };
            }

            var user = _mapper.Map<StoreUser>(userForRegistration);
            var result = await _userManager.CreateAsync(user, userForRegistration.Password);
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
            var userEmail = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(userEmail))
            {
                throw new MissingClaimInTokenException(ClaimTypes.Email);
            }

            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
                throw new RequestForUserException($"User with email: {userEmail} - not found.");

            user.IsDeleteRequested = true;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                throw new UserModificationException("UpdateAsync", user.Id);

            return true;
        }

        public async Task<bool> SubmitDeletion(string Id)
        {
            StoreUser user = await _userManager.FindByIdAsync(Id);
            if (user is null)
            {
                throw new RequestForUserException("Can't find user");
            }

            if (user.IsDeleteRequested is not true)
            {
                throw new InvalidOperationException("You can't delete active user");
            }

            var deleteResul = await _userManager.DeleteAsync(user);
            if (!deleteResul.Succeeded)
                throw new UserModificationException("DeleteAsync", user.Id);

            return true;
        }

        private SigningCredentials GetSigningCredentials()
        {
            var key = Encoding.UTF8.GetBytes(_jwtSettings["securityKey"]);
            var secret = new SymmetricSecurityKey(key);

            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        private List<Claim> GetClaims(StoreUser user)
        {
            var claims = new List<Claim>
      {
        new Claim(ClaimTypes.Name, user.Email)
      };

            return claims;
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var tokenOptions = new JwtSecurityToken(
              issuer: _jwtSettings["validIssuer"],
              audience: _jwtSettings["validAudience"],
              claims: claims,
              expires: DateTime.Now.AddMinutes(Convert.ToDouble(_jwtSettings["expiryInMinutes"])),
              signingCredentials: signingCredentials);

            return tokenOptions;
        }
    }
}
