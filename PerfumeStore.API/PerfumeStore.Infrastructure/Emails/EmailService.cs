using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using PerfumeStore.Domain.StoreUsers;
using System.Text;

namespace PerfumeStore.Infrastructure.Emails
{
    public class EmailService : IEmailService
    {
        private readonly IEmailSender _emailSender;
        private readonly UserManager<StoreUser> _userManager;
        private readonly IUrlHelper _urlHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EmailService(IEmailSender emailSender, UserManager<StoreUser> userManager, IUrlHelper urlHelper, IHttpContextAccessor httpContextAccessor)
        {
            _emailSender = emailSender;
            _userManager = userManager;
            _urlHelper = urlHelper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task SendActivationLink(StoreUser user)
        {
            string subject = "Activate you account";
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var confirmationLink = _urlHelper.Action(
              action: "ConfirmEmail",
              controller: "User",
              values: new { userId = user.Id, token = encodedToken },
              protocol: _httpContextAccessor.HttpContext.Request.Scheme);

            string message = $@"
          <h2>Hello {user.UserName},</h2>
          <p>We invite you to start using our service.</p>
          <a href='{confirmationLink}' style='display: inline-block; padding: 10px 20px; background-color: #3498db; color: white; text-decoration: none; border-radius: 5px; margin-top: 20px;'>Activate Account</a>
          <p style='margin-top: 20px;'>Instead, you can copy/paste this link into your browser:</p>
          <div style='background-color: #f3f3f3; padding: 10px; border-radius: 5px;'>
              <a href='{confirmationLink}' style='color: #3498db; text-decoration: none;'>{confirmationLink}</a>
          </div>";
            await _emailSender.SendEmailAsync(user.Email, subject, message);
        }

        public async Task ConfirmEmail(string userId, string encodedEmailToken)//TODO: exceptions are stupid
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"The User with Id: {user.Id} was not found.");

            var decodedTokenBytes = WebEncoders.Base64UrlDecode(encodedEmailToken);
            var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (!result.Succeeded)
                throw new KeyNotFoundException($"Email confirmation failed");
        }
    }
}
