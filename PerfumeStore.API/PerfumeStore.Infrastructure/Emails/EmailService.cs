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
using PerfumeStore.Domain.Core.DTO;
using PerfumeStore.Domain.StoreUsers;
using System.Text;

namespace PerfumeStore.Infrastructure.Emails
{
    public class EmailService : IEmailService
    {
        private readonly IEmailSender _emailSender;
        private readonly IUrlHelper _urlHelper;
        private readonly IHttpContextService _httpContextService;

        public EmailService(
            IEmailSender emailSender,
            IUrlHelper urlHelper, 
            IHttpContextService httpContextService
            ) 
        {
            _emailSender = emailSender;
            _urlHelper = urlHelper;
            _httpContextService = httpContextService;
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
Thank you for shopping
              <a href='{confirmationLink}' style='display: inline-block; padding: 10px 20px; background-color: #3498db; color: white; text-decoration: none; border-radius: 5px; margin-top: 20px;'>Activate Account</a>
              <p style='margin-top: 20px;'>Instead, you can copy/paste this link into your browser:</p>
              <div style='background-color: #f3f3f3; padding: 10px; border-radius: 5px;'>
                  <a href='{confirmationLink}' style='color: #3498db; text-decoration: none;'>{confirmationLink}</a>
              </div>";
            await _emailSender.SendEmailAsync(userDetails.UserEmail, subject, message);
        }

        public async Task SendOrderSummary(OrderResponse orderResponse)
        {
            string subject = "Order Summary";
            int rowIndex = 0;
            string message = $@"
                <h2>Hello {orderResponse.ShippingDetil.FirstName}, here is  your order summary, thank you for shopping!</h2>

                <table>
                    <tr style='background-color: grey;'>
                        <th colspan='4'>ORDER SUMMARY</th>
                    </tr>
                    <tr>
                        <th>Product name</th>
                        <th>Product unit price</th>
                        <th>Product quantity</th>
                        <th>Product total price</th>  
                    </tr>
            ";

            foreach (CheckCartDto product in orderResponse.AboutProductsInCart)
            {
                if (rowIndex % 2 == 0)
                {
                    message += $@"
                        <tr>
                            <td>PRODUCT NAME</td>
                            <td>{product.ProductUnitPrice}</td>
                            <td>{product.Quantity}</td>
                            <td>{product.ProductUnitPrice} X {product.Quantity} = {product.ProductTotalPrice}</td>
                        </tr>
                    ";
                }
                else
                {
                    message += $@"
                        <tr style='background-color: gray;'>
                            <td>PRODUCT NAME</td>
                            <td>{product.ProductUnitPrice}</td>
                            <td>{product.Quantity}</td>
                            <td>{product.ProductUnitPrice} X {product.Quantity} = {product.ProductTotalPrice}</td>
                        </tr>
                    ";
                }

                rowIndex++;
            }
            message += $@"
                <tr><td colspan='4'><strong>Shipping details</strong></td></tr>
                <tr><td>First name:</td><td>{orderResponse.ShippingDetil.FirstName}</td></tr>
                <tr><td>Last name:</td><td>{orderResponse.ShippingDetil.LastName}</td></tr>
                <tr><td>Email:</td><td>{orderResponse.ShippingDetil.Email}</td></tr>
                <tr><td>Phone:</td><td>{orderResponse.ShippingDetil.PhoneNumber}</td></tr>
                <tr><td>Address:</td><td>{orderResponse.ShippingDetil.Street} {orderResponse.ShippingDetil.StreetNumber}{(string.IsNullOrEmpty(orderResponse.ShippingDetil.HomeNumber) ? "" : "/" + orderResponse.ShippingDetil.HomeNumber)}, {orderResponse.ShippingDetil.PostCode}, {orderResponse.ShippingDetil.City}</td></tr>
            ";

            message += "</table>";

            await _emailSender.SendEmailAsync(orderResponse.ShippingDetil.Email, subject, message);
        }

        public string DecodeBaseUrlToken(string encodedEmailToken)
        {
            var decodedTokenBytes = WebEncoders.Base64UrlDecode(encodedEmailToken);
            var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

            return decodedToken;
        }
    }
}
