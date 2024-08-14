using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Store.Application.Contracts.Azure.Options;
using Store.Application.Contracts.JwtToken;
using Store.Application.Contracts.JwtToken.Models;
using Store.Application.Shared.Enums;
using Store.Domain.Abstractions;
using Store.Domain.StoreUsers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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
        IOptions<KeyVaultOptions> keyVaultOptions
        )
    {
        _next = next;
        _jwtOptions = jwtOptions.Value;
        _serviceProvider = serviceProvider;
        _logger = logger;
        _keyVaultOptions = keyVaultOptions.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var jwtToken = context.Request.Cookies[CookieNames.AuthCookie.ToString()];
        var refreshToken = context.Request.Cookies[CookieNames.RefreshCookie.ToString()];

        if (!string.IsNullOrEmpty(jwtToken))
        {
            using var scope = _serviceProvider.CreateScope();
            var tokenHandler = scope.ServiceProvider.GetRequiredService<JwtSecurityTokenHandler>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<StoreUser>>();
            var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();

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
                var principal = tokenHandler.ValidateToken(jwtToken, validationParameters, out var securityToken);
                var jwtSecurityToken = securityToken as JwtSecurityToken;

                if (jwtSecurityToken != null && jwtSecurityToken.ValidTo < DateTime.UtcNow)
                {
                    var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                    if (userId != null)
                    {
                        var user = await userManager.FindByIdAsync(userId);
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
                                    Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.JwtTokenExpirationInHours),
                                    IsEssential = true,
                                    SameSite = SameSiteMode.None
                                };

                                var refreshCookieOptions = new CookieOptions
                                {
                                    HttpOnly = true,
                                    Secure = true,
                                    Expires = DateTime.UtcNow.AddHours(_jwtOptions.RefreshTokenExpirationInHours),
                                    IsEssential = true,
                                    SameSite = SameSiteMode.None
                                };

                                context.Response.Cookies.Append(CookieNames.AuthCookie.ToString(), newJwtTokenResult.Value, authCookieOptions);
                                context.Response.Cookies.Append(CookieNames.RefreshCookie.ToString(), newRefreshTokenResult.Value, refreshCookieOptions);

                                var newPrincipal = tokenHandler.ValidateToken(newJwtTokenResult.Value, validationParameters, out _);
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
