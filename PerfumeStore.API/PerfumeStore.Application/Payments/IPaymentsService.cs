using PerfumeStore.Application.Abstractions.Result.Shared;
using PerfumeStore.Application.Shared.DTO.Request;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
