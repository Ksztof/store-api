using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Domain.Enums
{
    public enum OrderStatusE
    {
        New,
        Processing,
        Paid,
        Shipped,
        Delivered,
        Completed,
        Cancelled,
        Returned,
        Refunded,
    }
}
