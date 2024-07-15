using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Application.Shared.DTO.Request
{
    public class ConfirmPaymentDtoApp
    {
        public PaymentIntent PaymentIntent { get; set; }
    }
}
