using PerfumeStore.Application.Shared.DTO;
using PerfumeStore.Application.Shared.DTO.Response;

namespace PerfumeStore.Application.Contracts.Email
{
    public interface IEmailService
    {
        public Task SendActivationLink(UserDetailsForActivationLinkDto userDetails, string encodedToken);

        public string DecodeBaseUrlToken(string encodedEmailToken);

        public Task SendOrderSummary(OrderResponse orderResponse);
    }
}
