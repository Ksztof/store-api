using Store.Application.Orders.Dto.Response;
using Store.Application.Users.Dto;
using Store.Domain.Abstractions;

namespace Store.Application.Contracts.Email;

public interface IEmailService
{
    public Task<Result> SendActivationLink(UserDetailsForActivationLinkDto userDetails, string encodedToken);
    public string DecodeBaseUrlToken(string encodedEmailToken);
    public Task SendOrderSummary(OrderResponseDto orderResponse);
}
