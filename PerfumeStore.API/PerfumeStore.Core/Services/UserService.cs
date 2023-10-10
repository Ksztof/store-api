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
using Microsoft.IdentityModel.Tokens;

namespace PerfumeStore.Core.Services
{
  public class UserService : IUserService
  {
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IConfigurationSection _jwtSettings;

    public UserService(UserManager<IdentityUser> userManager, IConfiguration configuration)
    {
      _userManager = userManager;
      _configuration = configuration;
      _jwtSettings = _configuration.GetSection("JwtSettings");
    }

    public async Task<AuthResponseDto> Login(UserForAuthenticationDto userForAuthentication)
    {
      var user = await _userManager.FindByNameAsync(userForAuthentication.Email);
      if (user == null || !await _userManager.CheckPasswordAsync(user, userForAuthentication.Password))
      {
        AuthResponseDto failedResponse = new AuthResponseDto { ErrorMessage = "Invalid Authentication" };
        return failedResponse;
      }

      var signingCredentials = GetSigningCredentials();
      var claims = GetClaims(user);
      var tokenOptions = GenerateTokenOptions(signingCredentials, claims);
      var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

      AuthResponseDto authResponse = null;

      return authResponse;
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
