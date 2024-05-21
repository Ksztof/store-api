using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Application.Shared.DTO.Request
{
    public class PayWithCardDtoApp
    {
        public string PaymentMethodId { get; set; }
        public int Amount { get; set; }
        public string Currency { get; set; }
    }
}
