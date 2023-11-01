﻿using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using PerfumeStore.Core.DTOs.Request;
using PerfumeStore.Core.DTOs.Response;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;
using PerfumeStore.Domain.Models;

namespace PerfumeStore.Core.Services
{
  public class UserService : IUserService
  {
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IConfigurationSection _jwtSettings;
    private readonly ITokenService _tokenService;
    private readonly IUrlHelper _urlHelper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IEmailSender _emailSender;


    public UserService(UserManager<IdentityUser> userManager, IConfiguration configuration, ITokenService tokenService, IUrlHelper urlHelper, IHttpContextAccessor httpContextAccessor)
    {
      _userManager = userManager;
      _configuration = configuration;
      _jwtSettings = _configuration.GetSection("JwtSettings");
      _tokenService = tokenService;
      _urlHelper = urlHelper;
      _httpContextAccessor = httpContextAccessor;
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

      var tokenResponse = await _tokenService.GetToken("PerfumeStore.read");

      if (tokenResponse.IsError)
      {
        AuthResponseDto failedResponse = new AuthResponseDto { ErrorMessage = "Error obtaining token" };
        return failedResponse;
      }

      AuthResponseDto authResponse = new AuthResponseDto
      {
        IsAuthSuccessful = true,
        Token = tokenResponse.AccessToken
      };

      return authResponse;
    }

    public async Task<RegistrationResponseDto> RegisterUser(UserForRegistrationDto userForRegistration)
    {
      var user = new IdentityUser {UserName = userForRegistration.UserName, Email = userForRegistration.Email };
      var result = await _userManager.CreateAsync(user, userForRegistration.Password);
      if (!result.Succeeded)
      {
        var errors = result.Errors.Select(e => e.Description);
        return new RegistrationResponseDto { Errors = errors, IsSuccessfulRegistration = false };
      }

      var userName = userForRegistration.UserName;
      var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
      var confirmationLink = _urlHelper.Action(
        action: "ConfirmEmail",
        controller: "User",
        values: new { userId = user.Id, token = token },
        protocol: _httpContextAccessor.HttpContext.Request.Scheme);
      var message = new Message(new string[] { user.Email }, "Confirmation email link", confirmationLink, null, userName);
      await _emailSender.SendEmailAsync(message);

      return new RegistrationResponseDto { IsSuccessfulRegistration = true };
    }

    public async Task<bool> ConfirmEmail(string userId, string token)
    {
      if (userId == null || token == null)
      {
        return false;
      }

      var user = await _userManager.FindByIdAsync(userId);
      if (user == null)
      {
        return false;
      }

      var result = await _userManager.ConfirmEmailAsync(user, token);
      if (result.Succeeded)
      {
        return true;
      }

      return false;
    }

    private SigningCredentials GetSigningCredentials()
    {
      var key = Encoding.UTF8.GetBytes(_jwtSettings["securityKey"]);
      var secret = new SymmetricSecurityKey(key);

      return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }
    private List<Claim> GetClaims(IdentityUser user)
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
