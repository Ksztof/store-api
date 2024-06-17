using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Application.Abstractions.Result.Payment
{
    public static class PaymentErrors
    {
        public static readonly Error MissingCartIdCookie = new ("User.MissingCookieWithCartId", "Cookie with cart Id is missing");
    }
}
