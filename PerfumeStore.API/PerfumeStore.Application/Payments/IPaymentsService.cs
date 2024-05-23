using PerfumeStore.Application.Abstractions.Result.Result;
using PerfumeStore.Application.Shared.DTO.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Application.Payments
{
    public interface IPaymentsService
    {
        public Task<Result> PayWithCardAsync(PayWithCardDtoApp form);
        public Task<Result> VerifyPaymentAsync();
    }
}
