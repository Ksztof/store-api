using PerfumeStore.Domain.EnumsEtc;
using PerfumeStore.Domain.ShippingDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Application.DTOs.Response
{
    public class OrdersResDto
    {
        public string Status { get; set; }

        public IEnumerable<CartLineResponse> CartLineResponse { get; set; }
        public ShippingInfo ShippingInfo { get; set; } 
    }
}
