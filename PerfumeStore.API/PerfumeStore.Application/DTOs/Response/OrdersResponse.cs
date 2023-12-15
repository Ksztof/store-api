using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Application.DTOs.Response
{
    public class OrdersResponse
    {
        public IEnumerable<OrderResponse> Orders { get; set; }   
    }
}
