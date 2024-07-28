using PerfumeStore.Application.Shared.DTO;
using PerfumeStore.Application.Shared.DTO.Response;
using PerfumeStore.Domain.Abstractions;

namespace PerfumeStore.Application.Contracts.Email
{
    public interface IEmailService
    {
        public Task<Result> SendActivationLink(UserDetailsForActivationLinkDto userDetails, string encodedToken);

        public string DecodeBaseUrlToken(string encodedEmailToken);

        public Task SendOrderSummary(OrderResponse orderResponse);
    }
}
