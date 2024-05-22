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
        public Task PayWithCardAsync(PayWithCardDtoApp form);
        public Task VerifyPaymentAsync();
    }
}
