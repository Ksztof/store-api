using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Store.Application.Contracts.ContextHttp;
using Store.Application.Contracts.Email;
using Store.Application.Orders.Dto.Response;
using Store.Application.Users.Dto;
using Store.Domain.Abstractions;
using Store.Domain.Carts.Dto.Response;
using System.Text;

namespace Store.Infrastructure.Services.Email
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

        public async Task<Result> SendActivationLink(UserDetailsForActivationLinkDto userDetails, string encodedToken)
        {
            string subject = "Activate you account";

            Result<string> result = _httpContextService.GetActualProtocol();
            if (result.IsFailure)
            {
                return Result.Failure(result.Error);
            }

            var confirmationLink = _urlHelper.Action(
              action: "ConfirmEmail",
              controller: "User",
              values: new { userId = userDetails.UserId, token = encodedToken },
              protocol: result.Value);

            string message = $@"
              <h2>Hello {userDetails.UserName},</h2>
              <p>We invite you to start using our service.</p>
              <a href='{confirmationLink}' style='display: inline-block; padding: 10px 20px; background-color: #3498db; color:
                white; text-decoration: none; border-radius: 5px; margin-top: 20px;'>Activate Account</a>
              <p style='margin-top: 20px;'>Instead, you can copy/paste this link into your browser:</p>
              <div style='background-color: #f3f3f3; padding: 10px; border-radius: 5px;'>
                  <a href='{confirmationLink}' style='color: #3498db; text-decoration: none;'>{confirmationLink}</a>
              </div>";
            await _emailSender.SendEmailAsync(userDetails.UserEmail, subject, message);

            return Result.Success();
        }

        public async Task SendOrderSummary(OrderResponse orderResponse)
        {
            string subject = "Order Summary";
            int rowIndex = 0;
            string message = $@"
                <html>
                    <body style='font-family: Arial, sans-serif; font-size: 16px; background-color: #f4f4f4; margin: 0; padding: 20px;'>
                        <div style='text-align: center; margin-bottom: 20px;'>
                            <h2 style='color: #333;'>Hello {orderResponse.ShippingDetil.FirstName}, here is your order summary, thank you for shopping!</h2>
                        </div>
                        <div style='background: #fff; border-radius: 10px; overflow: hidden; box-shadow: 0 4px 8px rgba(0,0,0,0.1);'>
                            <table style='width: 100%; border-collapse: collapse; margin: 0; font-size: 0.9em;'>
                                <thead>
                                    <tr style='background-color: #7B68EE; color: #ffffff; text-align: center;'>
                                        <th style='padding: 15px;'>Product name</th>
                                        <th style='padding: 15px;'>Product unit price</th>
                                        <th style='padding: 15px;'>Product quantity</th>
                                        <th style='padding: 15px;'>Product total price</th>  
                                    </tr>
                                </thead>
                                <tbody>";
            foreach (CheckCartDomRes product in orderResponse.AboutProductsInCart)
            {
                string backgroundColor = rowIndex % 2 == 0 ? "#ffffff" : "#f2f2f2";
                message += $@"
                                    <tr style='background-color: {backgroundColor};'>
                                        <td style='padding: 10px; text-align: center;'>{product.ProductName}</td>
                                        <td style='padding: 10px; text-align: center;'>{product.ProductUnitPrice:C}</td>
                                        <td style='padding: 10px; text-align: center;'>{product.Quantity}</td>
                                        <td style='padding: 10px; text-align: center;'>{product.ProductUnitPrice:C} X {product.Quantity} szt. = {product.ProductTotalPrice:C}</td>
                                    </tr>";
                rowIndex++;
            }
            message += $@"
                                </tbody>
                            </table>
                        </div>
                        <div style='margin-top: 20px; padding: 15px; background: #fff; border-radius: 10px; box-shadow: 0 4px 8px rgba(0,0,0,0.1);'>
                            <h4 style='color: #333;'>Shipping details</h4>
                            <p style='color: #666;'>First name: {orderResponse.ShippingDetil.FirstName}</p>
                            <p style='color: #666;'>Last name: {orderResponse.ShippingDetil.LastName}</p>
                            <p style='color: #666;'>Email: {orderResponse.ShippingDetil.Email}</p>
                            <p style='color: #666;'>Phone: {orderResponse.ShippingDetil.PhoneNumber}</p>
                            <p style='color: #666;'>Address: {orderResponse.ShippingDetil.Street} {orderResponse.ShippingDetil.StreetNumber}{(string.IsNullOrEmpty(orderResponse.ShippingDetil.HomeNumber) ? "" : " " + orderResponse.ShippingDetil.HomeNumber)}, {orderResponse.ShippingDetil.PostCode}, {orderResponse.ShippingDetil.City}</p>
                        </ div >
                    </ body >
                </ html > ";

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
