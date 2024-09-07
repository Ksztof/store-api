using Store.Application.Payments.Dto.Request;
using Store.Domain.Abstractions;

namespace Store.Application.Payments;

public interface IPaymentsService
{
    public Task<Result<string>> GetClientSecretAsync(GetClientSecretDtoApp form);
    public Task<Result> UpdatePaymentIntentAsync(UpdatePaymentIntentDtoApp form);
    public Task<Result> ConfirmPaymentAsync(ConfirmPaymentDtoApp form);
    public Task VerifyPaymentAsync();
}
