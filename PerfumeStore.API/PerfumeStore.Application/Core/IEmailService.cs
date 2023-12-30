using PerfumeStore.Application.DTOs;
using PerfumeStore.Application.DTOs.Response;

namespace PerfumeStore.Application.Core
{
    public interface IEmailService
    {
        public Task SendActivationLink(UserDetailsForActivationLinkDto userDetails, string encodedToken);

        public string DecodeBaseUrlToken(string encodedEmailToken);

        public Task SendOrderSummary(OrderResponse orderResponse);
    }
}
