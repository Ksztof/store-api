using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PerfumeStore.Application.Contracts.JwtToken;
using PerfumeStore.Application.Contracts.JwtToken.Models;
using PerfumeStore.Domain.Abstractions;
using PerfumeStore.Domain.StoreUsers;
using PerfumeStore.Infrastructure.Services.Cookies;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PerfumeStore.Infrastructure.Services.Tokens
{
    public class JwtTokenService : ITokenService
    {
        private readonly IOptions<JwtOptions> _jwtOptions;
        private readonly UserManager<StoreUser> _userManager;
        private readonly ICookieService _cookieService;

        public JwtTokenService(
            UserManager<StoreUser> userManager,
            IOptions<JwtOptions> jwtOptions,
            ICookieService coookiesService)
        {
            _userManager = userManager;
            _jwtOptions = jwtOptions;
            _cookieService = coookiesService;
        }

        public async Task<Result> IssueJwtToken(StoreUser user)
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

            string token = GenerateToken(authClaims);

            Result result = _cookieService.SetCookieWithToken(token);

            return result;
        }

        private string GenerateToken(IEnumerable<Claim> claims)
        {
            string securityKeyString = _jwtOptions.Value.SecurityKey;
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKeyString));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _jwtOptions.Value.ValidIssuer,
                Audience = _jwtOptions.Value.ValidAudience,
                Expires = DateTime.UtcNow.AddHours(3),
                SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(claims)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public Result RemoveAuthCookie()
        {
            var expiredCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTimeOffset.Now.AddDays(-1),
                IsEssential = false,
                SameSite = SameSiteMode.None,
            };

            Result result = _cookieService.SetExpiredAuthToken(expiredCookieOptions);
            
            return result;
        }
    }
}
