using PerfumeStore.Application.Shared.DTO.Request;
using PerfumeStore.Domain.Abstractions;

namespace PerfumeStore.Application.Payments
{
    public interface IPaymentsService
    {
        public Task<Result<string>> GetClientSecretAsync(GetClientSecretDtoApp form);
        public Task<Result> UpdatePaymentIntentAsync(UpdatePaymentIntentDtoApp form);
        public Task<Result> ConfirmPaymentAsync(ConfirmPaymentDtoApp form);
        public Task VerifyPaymentAsync();
    }
}
