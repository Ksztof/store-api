using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Store.Application.Contracts.Azure.Options;
using Store.Application.Contracts.JwtToken;
using Store.Application.Contracts.JwtToken.Models;
using Store.Application.Users.Enums;
using Store.Domain.Abstractions;
using Store.Domain.StoreUsers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Store.Infrastructure.Middlewares;

public class JwtRefreshMiddleware
{
    private readonly RequestDelegate _next;
    private readonly JwtOptions _jwtOptions;
    private readonly KeyVaultOptions _keyVaultOptions;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<JwtRefreshMiddleware> _logger;

    public JwtRefreshMiddleware(
        RequestDelegate next,
        IOptions<JwtOptions> jwtOptions,
        IServiceProvider serviceProvider,
        ILogger<JwtRefreshMiddleware> logger,
        IOptions<KeyVaultOptions> keyVaultOptions)
    {
        _next = next;
        _jwtOptions = jwtOptions.Value;
        _serviceProvider = serviceProvider;
        _logger = logger;
        _keyVaultOptions = keyVaultOptions.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string? jwtToken = context.Request.Cookies[CookieNames.AuthCookie.ToString()];
        string? refreshToken = context.Request.Cookies[CookieNames.RefreshCookie.ToString()];

        if (!string.IsNullOrEmpty(jwtToken))
        {
            using IServiceScope scope = _serviceProvider.CreateScope();

            JwtSecurityTokenHandler tokenHandler = scope.ServiceProvider.GetRequiredService<JwtSecurityTokenHandler>();
            UserManager<StoreUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<StoreUser>>();
            ITokenService tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_keyVaultOptions.SecurityKey)),
                ValidateIssuer = true,
                ValidIssuer = _jwtOptions.ValidIssuer,
                ValidateAudience = true,
                ValidAudience = _jwtOptions.ValidAudience,
                ValidateLifetime = false,
            };

            try
            {
                ClaimsPrincipal principal = tokenHandler.ValidateToken(jwtToken, validationParameters, out var securityToken);
                JwtSecurityToken? jwtSecurityToken = securityToken as JwtSecurityToken;

                if (jwtSecurityToken != null && jwtSecurityToken.ValidTo < DateTime.UtcNow)
                {
                    var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                    if (userId != null)
                    {
                        StoreUser user = await userManager.FindByIdAsync(userId);

                        if (user != null && user.RefreshToken == refreshToken && user.RefreshTokenExpiryTime > DateTime.UtcNow)
                        {
                            Result<string> newJwtTokenResult = await tokenService.IssueJwtToken(user);
                            Result<string> newRefreshTokenResult = await tokenService.IssueRefreshToken(user);

                            if (newJwtTokenResult.IsSuccess && newRefreshTokenResult.IsSuccess)
                            {
                                var authCookieOptions = new CookieOptions
                                {
                                    HttpOnly = true,
                                    Secure = true,
                                    Expires = DateTime.UtcNow.AddHours(_jwtOptions.JwtTokenExpirationInHours),
                                    IsEssential = false,
                                    SameSite = SameSiteMode.None
                                };

                                var refreshCookieOptions = new CookieOptions
                                {
                                    HttpOnly = true,
                                    Secure = true,
                                    Expires = DateTime.UtcNow.AddHours(_jwtOptions.RefreshTokenExpirationInHours),
                                    IsEssential = false,
                                    SameSite = SameSiteMode.None
                                };

                                context.Response.Cookies.Append(CookieNames.AuthCookie.ToString(), newJwtTokenResult.Value, authCookieOptions);
                                context.Response.Cookies.Append(CookieNames.RefreshCookie.ToString(), newRefreshTokenResult.Value, refreshCookieOptions);

                                ClaimsPrincipal newPrincipal = tokenHandler.ValidateToken(newJwtTokenResult.Value, validationParameters, out _);
                                context.User = newPrincipal;

                                context.Items["NewAuthToken"] = newJwtTokenResult.Value;
                            }
                        }
                    }
                }
                else
                {
                    context.User = principal;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while processing JWT tokens: {ex.Message}");
            }
        }

        await _next(context);
    }
}
