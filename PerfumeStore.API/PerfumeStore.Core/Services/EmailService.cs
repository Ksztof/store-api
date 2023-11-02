using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PerfumeStore.Core.CustomExceptions;
using PerfumeStore.Domain.DbModels;

namespace PerfumeStore.Core.Services
{
  public class EmailService : IEmailService
  {
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly IUrlHelper _urlHelper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public EmailService(UserManager<IdentityUser> userManager, IEmailSender emailSender, IUrlHelper urlHelper, IHttpContextAccessor httpContextAccessor)
    {
      _userManager = userManager;
      _emailSender = emailSender;
      _urlHelper = urlHelper;
      _httpContextAccessor = httpContextAccessor;
    }

    public async Task SendActivationEmailAsync(IdentityUser user)
    {
      try
      {
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var confirmationLink = _urlHelper.Action(
          "ConfirmEmail",
          "User",
          new { userId = user.Id, token = token },
          _httpContextAccessor.HttpContext.Request.Scheme);
        string message = $@"
          <h2>Hello {user.UserName},</h2>
          <p>We invite you to start using our service.</p>
          <a href='{confirmationLink}' style='display: inline-block; padding: 10px 20px; background-color: #3498db; color: white; text-decoration: none; border-radius: 5px; margin-top: 20px;'>Activate Account</a>
          <p style='margin-top: 20px;'>Instead, you can copy/paste this link into your browser:</p>
          <div style='background-color: #f3f3f3; padding: 10px; border-radius: 5px;'>
              <a href='{confirmationLink}' style='color: #3498db; text-decoration: none;'>{confirmationLink}</a>
          </div>";

        await _emailSender.SendEmailAsync(
          user.Email,
          "Account activation",
          message);
      }
      catch (Exception e)
      {
        throw new InvalidOperationException("Can't send an email", e);
      }
    }

    public async Task ConfirmEmail(string userId, string token)//TODO: exceptions are stupid
    {
      var user = await _userManager.FindByIdAsync(userId);
      if (user == null)
        throw new KeyNotFoundException($"The User with Id: {user.Id} was not found.");

      var result = await _userManager.ConfirmEmailAsync(user, token);

      if (!result.Succeeded)
        throw new KeyNotFoundException($"Email confirmation failed");
    }
  }
}
