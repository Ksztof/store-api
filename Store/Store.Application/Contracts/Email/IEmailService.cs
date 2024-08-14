using Store.Application.Shared.DTO;
using Store.Application.Shared.DTO.Response;
using Store.Domain.Abstractions;

namespace Store.Application.Contracts.Email
{
    public interface IEmailService
    {
        public Task<Result> SendActivationLink(UserDetailsForActivationLinkDto userDetails, string encodedToken);

        public string DecodeBaseUrlToken(string encodedEmailToken);

        public Task SendOrderSummary(OrderResponse orderResponse);
    }
}
