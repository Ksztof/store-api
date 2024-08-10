using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PerfumeStore.Application.Contracts.JwtToken;
using PerfumeStore.Application.Contracts.JwtToken.Models;
using PerfumeStore.Domain.Abstractions;
using PerfumeStore.Domain.StoreUsers;
using PerfumeStore.Infrastructure.Services.Cookies;
using PerfumeStore.Application.Shared.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using PerfumeStore.Application.Contracts.Azure.Options;

namespace PerfumeStore.Infrastructure.Services.Tokens
{
    public class JwtTokenService : ITokenService
    {
        private readonly JwtOptions _jwtOptions;
        private readonly KeyVaultOptions _keyVaultOptions;
        private readonly UserManager<StoreUser> _userManager;
        private readonly ICookieService _cookieService;

        public JwtTokenService(
            UserManager<StoreUser> userManager,
            IOptions<JwtOptions> jwtOptions,
            ICookieService coookiesService,
            IOptions<KeyVaultOptions> keyVaultOptions
)
        {
            _userManager = userManager;
            _jwtOptions = jwtOptions.Value;
            _cookieService = coookiesService;
            _keyVaultOptions = keyVaultOptions.Value;
        }

        public async Task<Result<string>> IssueJwtToken(StoreUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            string token = GenerateJwtToken(authClaims);

            Result result = _cookieService.SetCookieWithJwtToken(token);
            if (result.IsFailure)
            {
                return Result<string>.Failure(result.Error);
            }

            return Result<string>.Success(token);
        }

        private string GenerateJwtToken(IEnumerable<Claim> claims)
        {
            string securityKeyString = _keyVaultOptions.SecurityKey;
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKeyString));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _jwtOptions.ValidIssuer,
                Audience = _jwtOptions.ValidAudience,
                Expires = DateTime.UtcNow.AddHours(_jwtOptions.JwtTokenExpirationInHours),
                SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(claims)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public async Task<Result<string>> IssueRefreshToken(StoreUser user)
        {
            string refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddHours(_jwtOptions.RefreshTokenExpirationInHours);
            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                return Result<string>.Failure(UserErrors.FailedToUpdateUserWithRefreshToken);
            }

            Result result = _cookieService.SetCookieWithRefreshToken(refreshToken);

            if (result.IsFailure)
            {
                return Result<string>.Failure(result.Error);
            }

            return Result<string>.Success(refreshToken);
        }
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public Result RemoveAuthToken()
        {
            Result result = _cookieService.SetExpiredCookie(CookieNames.AuthCookie);

            return result;
        }

        public Result RemoveRefreshToken()
        {
            Result result = _cookieService.SetExpiredCookie(CookieNames.RefreshCookie);

            return result;
        }
    }
}
