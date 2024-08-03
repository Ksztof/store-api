using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PerfumeStore.Application.Contracts.JwtToken;
using PerfumeStore.Application.Contracts.JwtToken.Models;
using PerfumeStore.Application.Users;
using PerfumeStore.Domain.Abstractions;
using PerfumeStore.Domain.StoreUsers;
using PerfumeStore.Infrastructure.Services.Tokens;
using PerfumeStore.Infrastructure.Shared.Enums;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Infrastructure.Middlewares
{
    public class JwtRefreshMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IOptions<JwtOptions> _jwtOptions;
        private readonly IServiceProvider _serviceScopeFactory;
        private readonly ILogger<JwtRefreshMiddleware> _logger;

        public JwtRefreshMiddleware(
            RequestDelegate next,
            IOptions<JwtOptions> jwtOptions,
            IServiceProvider serviceScopeFactory,
            ILogger<JwtRefreshMiddleware> logger)

        {
            _next = next;
            _jwtOptions = jwtOptions;
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;

        }

        public async Task InvokeAsync(HttpContext context)
        {
            var jwtToken = context.Request.Cookies[CookieNames.AuthCookie.ToString()];
            var refreshToken = context.Request.Cookies[CookieNames.RefreshCookie.ToString()];

            if (!string.IsNullOrEmpty(jwtToken))
            {
                using IServiceScope scope = _serviceScopeFactory.CreateScope();
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Value.SecurityKey)),
                    ValidateIssuer = true,
                    ValidIssuer = _jwtOptions.Value.ValidIssuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtOptions.Value.ValidAudience,
                    ValidateLifetime = false,
                };

                try
                {
                    var principal = tokenHandler.ValidateToken(jwtToken, validationParameters, out var securityToken);
                    var jwtSecurityToken = securityToken as JwtSecurityToken;

                    if (jwtSecurityToken != null && jwtSecurityToken.ValidTo < DateTime.UtcNow)
                    {
                        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<StoreUser>>();

                        StoreUser? user = await userManager.FindByIdAsync(userId);

                        if (user == null)
                        {
                            throw new KeyNotFoundException($"User with ID '{userId}' was not found.");
                        }


                        if (user.RefreshToken == refreshToken && user.RefreshTokenExpiryTime > DateTime.UtcNow)
                        {
                            var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();

                            Result<string> newJwtTokenResult = await tokenService.IssueJwtToken(user);
                            Result<string> newRefreshTokenResult = await tokenService.IssueRefreshToken(user);

                            if (newJwtTokenResult.IsSuccess && newRefreshTokenResult.IsSuccess)
                            {
                                context.Response.Cookies.Append(CookieNames.AuthCookie.ToString(), newJwtTokenResult.Value, new CookieOptions { HttpOnly = true, Secure = true });
                                context.Response.Cookies.Append(CookieNames.RefreshCookie.ToString(), newRefreshTokenResult.Value, new CookieOptions { HttpOnly = true, Secure = true });

                                var newPrincipal = tokenHandler.ValidateToken(newJwtTokenResult.Value, validationParameters, out _);
                                context.User = newPrincipal;
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
                    _logger.LogError(ex, $"An error occurred while processing JWT tokens with message: {ex.Message}");
                }
            }
            await _next(context);
        }
    }
}