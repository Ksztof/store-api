using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Domain.DTOs.Request
{
    public class ProductModificationDom
    {
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
    }
}
