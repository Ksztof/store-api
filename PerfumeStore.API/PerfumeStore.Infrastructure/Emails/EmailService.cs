using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using PerfumeStore.Application.Core;
using PerfumeStore.Application.DTOs;
using PerfumeStore.Application.DTOs.Response;
using PerfumeStore.Application.HttpContext;
using PerfumeStore.Application.Users;
using PerfumeStore.Domain.Abstractions;
using PerfumeStore.Domain.StoreUsers;
using System.Text;

namespace PerfumeStore.Infrastructure.Emails
{
    public class EmailService : IEmailService
    {
        private readonly IEmailSender _emailSender;
        private readonly IUrlHelper _urlHelper;
        private readonly IHttpContextService _httpContextService;
        private readonly IUserService _userService;

        public EmailService(
            IEmailSender emailSender,
            UserManager<StoreUser> userManager, 
            IUrlHelper urlHelper, 
            IHttpContextService httpContextService,
            IUserService userService
            ) 
        {
            _emailSender = emailSender;
            _urlHelper = urlHelper;
            _httpContextService = httpContextService;
            _userService = userService;
        }

        public async Task SendActivationLink(UserDetailsForActivationLinkDto userDetails, string encodedToken)
        {
            string subject = "Activate you account";

            var confirmationLink = _urlHelper.Action(
              action: "ConfirmEmail",
              controller: "User",
              values: new { userId = userDetails.UserId, token = encodedToken },
              protocol: _httpContextService.GetActualProtocol());

            string message = $@"
              <h2>Hello {userDetails.UserName},</h2>
              <p>We invite you to start using our service.</p>
              <a href='{confirmationLink}' style='display: inline-block; padding: 10px 20px; background-color: #3498db; color: white; text-decoration: none; border-radius: 5px; margin-top: 20px;'>Activate Account</a>
              <p style='margin-top: 20px;'>Instead, you can copy/paste this link into your browser:</p>
              <div style='background-color: #f3f3f3; padding: 10px; border-radius: 5px;'>
                  <a href='{confirmationLink}' style='color: #3498db; text-decoration: none;'>{confirmationLink}</a>
              </div>";
            await _emailSender.SendEmailAsync(userDetails.UserEmail, subject, message);
        }

        public async Task SendOrderSummary(OrderResponse orderResponse)
        {
           /* string subject = "Order Summary";

            string message = $@"
                
            ";*/



        }

        public string DecodeBaseUrlToken(string encodedEmailToken)
        {
            var decodedTokenBytes = WebEncoders.Base64UrlDecode(encodedEmailToken);
            var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

            return decodedToken;
        }
    }
}
