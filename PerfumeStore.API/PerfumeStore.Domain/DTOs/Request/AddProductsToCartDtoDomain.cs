using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Domain.DTOs.Request
{
    public class AddProductsToCartDtoDomain
    {
        public ProductInCartDomain[] Products { get; set; }
    }
}
